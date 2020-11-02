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
1. 创建 Player 类  
Player类型是一个动态类，继承自ConnectionPlayerBase。Island.StandardLib将为您处理一切连接事务，在任何客户端成功连接后，会为您自动创建一个Player类的实例，每个客户端都会对应一个不同的Player实例。当然，如果您要做的是1对1的连接，那么整个程序中只有一个Player实例。  
a.您需要重写void PassCommand(ConnectCommand command)函数。这个函数的意义是处理来自客户端的消息，就像处理UI中的 OnKeyDown 一样，您必须保证函数运行时间尽可能短。您的代码在PassCommand中执行会使传输操作暂停，当这个函数执行结束时传输操作会恢复。例如，客户端向您发送了一副图片，您可以在此处处理。  
```cs
public class Player : ConnectionPlayerBase
{
    const int CMD_UPDATE_IMG = 0x1;  // 定义更新图片的指令的Name
    const int CMD_TEST = 0x2;        // 定义测试的指令的Name

    protected override void PassCommand(ConnectCommand command)
    {
        // 这个客户端向服务器发送了一条指令，指令的内容在 command 参数中
        // command.Name 一个int，用来保存指令名字，比如 "更新图片"，但是在这里，为了方便传输和存储，指令是一个数字。
        // command.Args 一个"数组"，用来保存数据，比如 "更新图片" 指令的数据就是一个图片。(其实这是 Island.StandardLib 提供的 StorableMultArray，可序列化可变长且不定项长数组，后续介绍)
        switch (command.Name)
        {
            case CMD_UPDATE_IMG:
                StorImage img = command.Args[0].As<StorImage>();  // 取数组第0项，将值转化为 StorImage （即可传输的图片类型)
                byte[] raw = img.Data;       // raw 即为传输中使用的已编码图像
                Image img = img.Image;       // img 即为解码后的 .Net 图像

                // Do somethings...
                // Such as UpdateImageToForm(img)...

                break;
            case CMD_TEST:
                Logger.Log(LogLevel.Default, command.Args[0].AsString());  // 收到客户端发来的测试消息，Log 出来
                CommandSendPool.AddCommand(CMD_TEST.CommandWithArgs("!!!Server Received Your Message!!!"));  // 在发回去，告诉客户端收到了
                // int.CommandWithArgs 是一个拓展函数（可以理解为语法糖），可以直接 0x2.CommandWithArgs(Data) 来创建一个指令，指令名0x2，数据Data。
                break;
        }
    }
}
```
2. 创建 Server 类  
Server类也是一个动态类，继承自 ConnectionServer<Player, LoginOrRegisterRequest>。在此泛型参数中，Player即为第一步中创建的Player类，LoginOrRegisterRequest是玩家登录时发送的登录包的数据模型，填写LoginOrRegisterRequest即使用Island.StandardLib自带的默认登录数据模型，您也可以填写EncryptedData<LoginOrRegisterRequest>来加密传输，也可以自定义这个登录数据模型的格式（后续详细讲解，在此处理解为数据模型即可）  
a.您需要重写LoginResult PassLogin(LoginOrRegisterRequest request)函数。这个函数的意义是处理登录请求，并判断是否让这个请求登录（即连接，反之直接T下线）。将此函数设置为必须重写的意义是，无论是否需要登录，您都应该验证请求是否来自您编写的客户端来保证安全。此函数返回是否通过登录请求，在本机测试时可直接 return LoginResult.Success; 来通过请求。  
b.您需要重写RegisterResult PassReg(LoginOrRegisterRequest request)函数。这个函数的意义是处理注册请求，您可以查看详细文档来定义您的注册流程，作为快速开始教程此处不展开讲解，不需要注册直接 return RegisterResult.ConnectionError; 。
```cs
public class Server : ConnectionServer<Player, LoginOrRegisterRequest>
{
    public static Server instance;  // 一般来说Server只有一个实例，所以可以使用单例模式

    public Server() : base("服务器监听的IP地址", 服务器监听的端口, 1, 1024 * 1024 * 1024)
    {
        instance = this;
    }

    protected override LoginResult PassLogin(LoginOrRegisterRequest request) => OnlinePlayers.Count == 0 ? LoginResult.Success : LoginResult.AlreadyOnline; // 仅允许一个客户端连接
    protected override RegisterResult PassReg(LoginOrRegisterRequest request) => RegisterResult.ConnectionError;
}
```
3. 启动你的服务器
你可以在任何地方启动你的服务器，只需要使用 new Server() 即可。例如：
```cs
public class Program
{
    static void Main(string[] args) => new Server();
}
```
到现在为止你已经创建好了服务器，如果你开启了控制台(生成命令行程序，或手动AllocConsole)，有没有看到那个伟大的Logo呢？笑）
# Client 项目
1. 创建 Client 类  
Client类是一个动态类，继承自 ConnectionClient  
a.您需要重写void PassCommand(ConnectCommand command)函数，含义与Player中相同。  
```cs
public class Client : ConnectionClient
{
    const int CMD_UPDATE_IMG = 0x1;  // 定义更新图片的指令的Name
    const int CMD_TEST = 0x2;        // 定义测试的指令的Name

    public Client(string addr, int port) : base(addr, port, 1, false, 1024 * 1024 * 1024) { }

    protected override void OnConnectionBegin()
    {
        base.OnConnectionBegin();
        Logger.Log("连接成功");
        CommandSendPool.AddCommand(CMD_TEST.CommandWithArgs("Hello server!"));
    }

    protected override void PassCommand(ConnectCommand command)
    {
        switch (command.Name)
        {
            case CMD_UPDATE_IMG:
                StorImage img = new StorImage(Image.FromXXX(xxx)); // 从Image生成StorImage
                CommandSendPool.AddCommand(CMD_UPDATE_IMG.CommandWithArgs(img));
                break;
            case CMD_TEST:
                Logger.Log(LogLevel.Default, command.Args[0].AsString());
                CommandSendPool.AddCommand(CMD_TEST.CommandWithArgs("!!!Client Received Your Message!!!"));
                break;
        }
    }
}
```
2. 启动你的客户端
```cs
public class Program
{
    static void Main(string[] args)
    {
        Client cl = new Client("服务器正在监听的地址", 服务器正在监听的端口);
        cl.ConnectAsLogin(0, " "); // 不需要账号密码
        while (true) Thread.Sleep(1);
    }
}
```
