# Island.StandardLib
A CSharp library for Client/Server Connection and Data Serialization, based by TCP/IP protocol.  
一个C#的C/S连接和数据序列化库，基于TCP/IP协议。
# 使用本项目的作品
PhySim Lab (iOS APP) 项目采用Island.StandardLib进行序列化并保存到存档文件。  
Natural Island (iOS APP) 项目采用Island.StandardLib进行C/S连接。  

# 快速开始
一般情况下，推荐至少创建三个项目。  
推荐创建的项目：  
* Server (服务器) 项目  
  ***需要引用 Island.StandardLib.dll 和您创建的 StandardDataStructure***  
  服务器项目用于描述服务器的行为，服务器程序开启监听端口，并可以同时接受和处理多个客户端连接。  
* Client (客户端) 项目  
  ***需要引用 Island.StandardLib.dll 和您创建的 StandardDataStructure***  
  客户端项目用于描述客户端的行为，一个客户端应该只能同时连接一个服务器。
* StandardDataStructure (共享数据模型库) 项目  
  ***需要引用 Island.StandardLib.dll***  
  **在这里介绍一下数据模型的概念：它在传输数据时是一个原子的概念，即传输时不可分割的数据，这个数据模型的大小可以为任意大小(当然，不能超过4GiB)，Island.StandardLib会动态处理TCP包的大小。例如，您可以将一些图片序列当作一个数据模型，然后直接传输这个序列，Island.StandardLib内部会将这个模型分割为很多个小包，然后逐个传输。**  
  当客户端或服务器向对方发送数据时，除了发送由 Island.StandardLib 定义的基本数据结构，还可以发送您定义的 **继承自IStorable** 的数据模型。关于如何自定义数据模型将在后续介绍。

# Island.StandardLib 使用的类型和函数命名规范
一切必须重写的函数都将以 PassXXX 命名，一切可选重写的函数都将以 OnXXX 命名。

# Server 项目
1. 创建 Player 类型  
Player类型是一个动态类，继承自ConnectionPlayerBase。Island.StandardLib将为您处理一切连接事务，在任何客户端成功连接后，会为您自动创建一个Player类的实例，每个客户端都会对应一个不同的Player实例。当然，如果您要做的是1对1的连接，那么整个程序中只有一个Player实例。  
您需要重写ConnectionPlayerBase的void PassCommand(ConnectCommand command)函数。这个函数的意义是处理来自客户端的消息，就像处理UI中的 OnKeyDown 一样，您必须保证函数运行时间尽可能短。您的代码在PassCommand中执行会使传输操作暂停，当这个函数执行结束时传输操作会恢复。例如，客户端向您发送了一副图片，您可以在此处处理。
