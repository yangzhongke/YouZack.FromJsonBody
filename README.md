# YouZack.FromJsonBody
Bind simple parameter of Action with HttpBody of Json format.
For ASP.Net Core MVC and ASP.Net Core WebAPI

···
public async Task<IActionResult> Test([FromJsonBody]string phoneNumber, [FromJsonBody]string test1, 
[FromJsonBody]int? age, [FromJsonBody] bool gender, [FromJsonBody] double salary);
···

Http Body:
{"phoneNumber":"110112119","age":18,"gender":true}

and ContentType of request should be application/json

Add following code to Startup **BEFORE app.UseEndpoints():**
···
app.UseFromJsonBody();//using YouZack.FromJsonBody;
···

Done!

To KEEP IT SIMPLE, YouZack.FromJsonBody only supports simple parameter types,like int,long,double,bool,Enum,string ,but does not support complex types,like Array,List,POCO etc.
If you want to use complex types, please go backwards to '[FromBody]' and ModelBinder.