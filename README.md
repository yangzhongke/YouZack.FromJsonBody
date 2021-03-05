[中文版文档](https://github.com/yangzhongke/YouZack.FromJsonBody/blob/master/README_CHS.md)

# YouZack.FromJsonBody
Bind simple parameters of Action with Json request.
For ASP.Net Core MVC and ASP.Net Core WebAPI

## Step One:

```
Install-Package YouZack.FromJsonBody
```

## Step Two:
```
public async Task<IActionResult> Test([FromJsonBody]string phoneNumber, [FromJsonBody]string test1, 
[FromJsonBody]int? age, [FromJsonBody] bool gender, [FromJsonBody] double salary);

public async Task<int> Post([FromJsonBody] int i1, [FromJsonBody] int i2);
```

## Step Three:
Http Body:
{"phoneNumber":"110112119","age":18,"gender":true}

and ContentType of request **SHOULD BE** application/json

Add following code to Startup **BEFORE app.UseEndpoints():**
```
app.UseFromJsonBody();//using YouZack.FromJsonBody;
```

Done!

To KEEP IT SIMPLE, YouZack.FromJsonBody only supports simple parameter types,like int,long,double,bool,Enum,string ,but does not support complex types,like Array,List,POCO etc.
If you want to use complex types, please go backwards to '[FromBody]' and ModelBinder.

[Detail of this project, how it works](https://www.reddit.com/r/dotnetcore/comments/lxio9n/the_best_way_to_get_value_from_json_request_on/)