[英文版文档](https://github.com/yangzhongke/YouZack.FromJsonBody/blob/master/README.md)

# YouZack.FromJsonBody
绑定Json请求到Action的简单类型参数。支持ASP.Net Core MVC和ASP.Net Core WebAPI。

## 第一步:

```
Install-Package YouZack.FromJsonBody
```

## 第二步:
```csharp
public async Task<IActionResult> Test([FromJsonBody]string phoneNumber, [FromJsonBody]string test1, 
[FromJsonBody]int? age, [FromJsonBody] bool gender, [FromJsonBody] double salary);

public async Task<int> Post([FromJsonBody] int i1, [FromJsonBody] int i2);
```

## 第三步:
Http 请求体:
{"phoneNumber":"110112119","age":18,"gender":true}

请求的 ContentType **必须是** application/json
在Startup.cs的**app.UseEndpoints()方法之前**加入如下代码：
```csharp
app.UseFromJsonBody();//using YouZack.FromJsonBody;
```

搞定!

为了保持简洁，YouZack.FromJsonBody只支持简单类型参数，比如int,long,double,bool,Enum,string，但是不支持复杂类型，比如Array,List,POCO等。
如果想要使用复杂类型，请继续使用传统的'[FromBody]' 

[项目的详细介绍以及原理介绍](https://www.toutiao.com/i6935675278383530533/)