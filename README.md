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
  **在这里介绍一下数据模型的概念：这个数据模型的大小可以为任意大小，Island.StandardLib会动态处理TCP包的大小。例如，您可以将一些图片序列当作一个数据模型，然后直接传输这个序列，Island.StandardLib内部会将这个模型分割为很多个小包，然后逐个传输。**  
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
到现在为止，你可以先指定监听地址为127.0.0.1（本机），启动你的服务器，再启动你的客户端，如果一切正确，你应该能看到服务器不断地打印 !!!Client Received Your Message!!! ，客户端不断地打印 !!!Server Received Your Message!!! ，恭喜你，你已经成功创建了一个服务器和客户端。
# StandardDataStructure 项目
此项目为启发式项目，不详细在教程中展开。  
你可能已经发现，
```cs
const int CMD_UPDATE_IMG = 0x1;  // 定义更新图片的指令的Name
const int CMD_TEST = 0x2;        // 定义测试的指令的Name
```
此代码在两个项目中都出现了。为了避免改动时只改了一方忘记改另一方，方便处理，所以创建 StandardDataStructure 存放共有的内容。具体类自行设计。  
此外，此项目中应该存放传输的数据模型。Island.StandardLib只提供了有限的常见数据模型，更多的模型需要您自己描述。有关介绍参见下一章。  
# Island.StandardLib 常用数据模型介绍
### SInt
*可序列化的 int ，是 int 基础类型的拓展，并可隐式转换*  
用法同 int   
### SBool
*可序列化的 bool ，是 bool 基础类型的拓展，并可隐式转换*  
用法同 bool   
### SString
*可序列化的 string ，是 string 基础类型的拓展，并可隐式转换*  
用法同 string   
***以上三个基础类型拓展主要用于需要IStorable的泛型参数中，由于使用这些类型序列化需要多占用4字节的存储空间（序列化机制决定的），所以仅在必要的情况下使用。***
### StorImage
*存储图像*  
初始化：StorImage img = new StorImage(<Image>);  
注意执行初始化时会自动将<Image>编码，此过程需要消耗短暂时间，注意时间复杂度。  
恢复: Image rimg = img.Image;  
注意获取 Image 属性时会自动将二进制数据解码，此过程需要消耗短暂时间，注意时间复杂度。  
| 成员 | 含义 |
| :----: | :----: |
| StorImage(Image img) | 编码整个图像并初始化 |
| byte *Data | Raw 格式的二进制已编码数据 |
| Image Image | 编码或解码数据 |  
### MultData
*万能存储类*  
可存储各种类型  
### StorableDictionary<TKey, TValue> where TKey : IStorable, new() where TValue : IStorable, new()
*可序列化的字典容器*  
用法同 Dictionary<TKey, TValue>  
### StorableFixedArray<T> where T : IStorable, new()
*可序列化的 固定项长 不定长度 的数组容器*  
用法同 List<T>
### StorableMultArray
*可序列化的 不定项长 不定长度 的数据容器*  
类似于 List<Object>，每一项可存储不同元素。等同于 StorableFixedArray<MultData> ，即每项存储的内容均为MultData，MultData的长度不固定，故从MultData获取数据时需指定类型，比如 T object = <MultData>.As<T>(); 或使用 <MultData>.AsXXX(); ，XXX为基本类型。
### MultiSizeData
*支持跨线程操作版MultData*  
实现更多操作  
### ConnectCommand
*传输时使用的序列化类型*  
可以使用 0x0.CommandWithArgs(args0, args1, ...) 来快速创建ConnectCommand
| 成员 | 含义 |
| :----: | :----: |
| int Name | 指令名称 |
| StorableMultArray Args | 指令数据 |  
### EncryptedData<DataType> : where DataType : IStorable, new()
*加密的数据*  
例如 EncryptedData<StorableDictionary<SString, SString>> 可表示一个加密的字典容器，传输时更安全。  
在加密和解密时需要指定 EncrypterBase 和 Key(如果需要)，Island.StandardLib提供了DES加密算法 RijndaelEncrypter。  
### Vector2 Vector2L Vector3 Vector3L ......
*N维向量*  
仅用于存储，不建议使用那些运算的函数，没有符号重载，很久之前写的不建议用不排除有错误。  
# Island.StandardLib 自定义可序列化数据模型  
在 Island.StandardLib 中，自定义可序列化函数需要继承 IStorable 接口，实现 void ReadFromData(DataStorage data) 和 void WriteToData(DataStorage data) 两个函数。  
以OOP入门的学生信息类为例，我们将它改造成可序列化的类。   
```cs
public class Student : IStorable
{
    public Student() { }   // 必须包含无参构造函数
    
    public string Name;    // 学生姓名
    public int Age;        // 学生年龄
    public StorableFixedArray<SInt> SubjectScores;   // 每科目的成绩，此处原为 int[] SubjectScores，是一个每项固定长度的数组，改造为 StorableFixedArray<> ，模板参数为 int，但模板参数需要使用可序列化的 int，即 SInt，故写作 StorableFixedArray<SInt>
    
    // 从 DataStorage 中读出数据，使用 data.Read(out xxx)
    public void ReadFromData(DataStorage data)
    {
        data.Read(out Name);
        data.Read(out Age);            // 此处使用 DataStorage::Read<T>(out T) where T : IStorable, new() 的重载，模板参数为 StorableFixedArray<SInt>
        data.Read(out SubjectScores);
    }
    
    // 将数据写入 DataStorage，使用 data.Write(xxx)
    // 注意：读写一定要顺序一致，否则会破坏读写过程
    public void WriteToData(DataStorage data)
    {
        data.Write(Name);
        data.Write(Age);
        data.Write(SubjectScores);
    }
    
    public int CalcTotalScore()
    {
        int value = 0;
        foreach (SInt it in SubjectScores) value += it; // StorableFixedArray<SInt> 和 int[] 的用法相似，只需要简单修改即可
        return value;
    }
}
```
### 序列化嵌套
我们继续改造 Student 类，并新增一个 Score 类来保存学生的ABC科目成绩。
```cs
public class Score : IStorable
{
    public int A, B, C;
    
    public Score() { }  // 必须包含无参构造函数
    
    public Score(int a, int b, int c)
    {
        A = a;
        B = b;
        C = c;
    }
    
    public int CalcTotalScore() => A + B + C;
    
    public void ReadFromData(DataStorage data)
    {
        data.Read(out A);
        data.Read(out B);
        data.Read(out C);
    }
    
    public void WriteToData(DataStorage data)
    {
        data.Write(A);
        data.Write(B);
        data.Write(C);
    }
}

public class Student : IStorable
{
    public Student() { }

    public string Name;    // 学生姓名
    public int Age;        // 学生年龄
    public Score SubjectScores;   // 每科目的成绩，我们将它改造为 Score 类型的成员。
    
    // 从 DataStorage 中读出数据，使用 data.Read(out xxx)
    public void ReadFromData(DataStorage data)
    {
        data.Read(out Name);
        data.Read(out Age);             // 此处依然使用 DataStorage::Read<T>(out T) where T : IStorable, new() 的重载，只不过当前的模板参数为 Score
        data.Read(out SubjectScores);
    }
    
    // 将数据写入 DataStorage，使用 data.Write(xxx)
    // 注意：读写一定要顺序一致，否则会破坏读写过程
    public void WriteToData(DataStorage data)
    {
        data.Write(Name);
        data.Write(Age);
        data.Write(SubjectScores);
    }
}
```
可见，序列化过程中可以嵌套其它的序列化类。
### 序列化继承
我们继续改造 Student 类，将分数分离出去。
```cs
public class Student : IStorable
{
    public Student() { }
    
    public string Name;    // 学生姓名
    public int Age;        // 学生年龄
    
    public void ReadFromData(DataStorage data)
    {
        data.Read(out Name);
        data.Read(out SubjectScores);
    }
    
    public void WriteToData(DataStorage data)
    {
        data.Write(Name);
        data.Write(SubjectScores);
    }
}

public class ScoredStudent : Student, IStorable
{
    public ScoredStudent() { }
    
    public StorableFixedArray<SInt> Scores;
    
    public void ReadFromData(DataStorage data)
    {
        base.ReadFromData(data);     // 让基类读取自己的信息
        data.Read(out Scores);       // 读取本类的信息
    }
    
    public void WriteToData(DataStorage data)
    {
        base.WriteToData(data);      // 让基类写入自己的信息
        data.Write(Scores);          // 写入本类的信息
    }
}
可见，实现序列化的类可以被继承，但子类若包含要保存的成员变量，子类也必须实现 IStorable 接口，然后在读写的时候先调用基类的读写函数，再读写自己的成员。
```

