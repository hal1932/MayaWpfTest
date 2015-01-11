import socket
import contextlib
import sys
import random
import threading

import maya.cmds as cmds
import maya.utils as utils


conn = None
instanceDic = {}


## MVVM での Model に相当するクラス、C# 側から呼び出される
class SceneInspector:
    def getNodeList(self):
        result = []
        for node in invoke(cmds.ls, transforms = True):
            result.append(node)
        return ",".join(result)

    def selectNode(self, node):
        invoke(cmds.select, node, replace = True)


## Model のインスタンスを生成する
def createInstance(className):
    global instanceDic, conn
    
    key = random.randint(0, 10000)
    instanceDic[key] = getattr(sys.modules[__name__], className)()
    sendResult(str(key))


## C# 側に結果を返す、実践では文字列化する前に受け取って MessagePack 等でパッキングすることを想定
def sendResult(result):
    global conn

    if result is None: result = ""
    conn.send("%d %s" % (len(result), result))


## コマンドや API をメインスレッドから呼び出すためのラッパ
def invoke(*args, **kwargs):
    return utils.executeInMainThreadWithResult(*args, **kwargs)


## Model のメソッドを呼び出すためのラッパ
def invokeMethod(instanceKey, method, args = None):
    global instanceDic
    
    instance = instanceDic[instanceKey]
    method = getattr(instance, method)
    
    result = ""
    if args is None:
        result = method()
    else:
        result = method(*args)
    
    sendResult(result)


## C# 側から送られてきたコマンドをパースして実行する
def parseCommand(commandStr):
    items = commandStr.split(" ")
    command = items[0]
    if command == "new":
        createInstance(items[1])
    elif command == "invoke":
        if len(items) == 3:
            invokeMethod(int(items[1]), items[2])
        else:
            invokeMethod(int(items[1]), items[2], items[3:])


## デーモンのメイン
class MainThread(threading.Thread):
    def run(self):
        global conn

        host = "localhost"
        port = 1111
        backlog = 10
        bufsize = 256
        
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        with contextlib.closing(sock):
            sock.bind((host, port))
            sock.listen(backlog)

            conn, _ = sock.accept()
            with contextlib.closing(conn):
                while True:
                    command = conn.recv(bufsize)
                    print "received: " + command
                    if command == "__exit__":
                        break
                    parseCommand(command)


MainThread().start()
