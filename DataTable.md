Fill data for Datatable from server side
==========
## Client side
### HTML
``` HTML
<table id="example"></table>
```
### Javascript
```Javascript
// Define các column
var columns = [{
    title: 'First name',
    mData: 'first_name'
  },{
    title: 'Last name',
    mData: 'last_name'
  }, {
    title: 'Full name'
    data: function(data){
      return data.first_name + " " + data.last_name
    }
  }
]

$('#example').DataTable({
  processing: true,
  serverSide: true,
  ajax: {
    type: "POST",
    contentType: "application/json",
    url: '/Category/GetDataTable',
    data: function (data) {
        // data response from server, should be struct like data above
        console.log(JSON.stringify(data));
        return JSON.stringify(data);
    }
  },
})
```
### POST data
When 
Action from DataTable will sent request to server by Post methods with params struct like below
```Javascript
{
  draw: 1,
  start: 0,
  length: 20,
  search: {
    value: "search key",
    regex: false
  },
  order: [
    {
      column: 1,
      dir: "asc",
    },
  ],
  columns: [{...}]
}
```
- `draw` sate of draw
- `start` position where data will be fetched
- `length` mumber of data (row) you wanto fetch per page
- `search` object include `value` is search key and `regex` (*true* | *false*) to defind how to use regex
- `order` array object include oder info of each column, include `column` is index of column on table and `dir` sortt type (*"asc"* | *"desc"*)
- `columns` array ofject include column info


## Server side
### *DataTableRequest* class
Base on post param data struct, we create a corresponding class

```c#
public class DataTableRequest
{
    public int draw { get; set; }
    public int start { get; set; }
    public int length { get; set; }
    // Not use yet, so wil be define by array object
    public List<object> columns { get; set; }
    public List<Order> order { get; set; }
    public Search search { get; set; }
    public int _ { get; set; }
}

public class Search
{
    public string value { get; set; }
    public string regex { get; set; }
}

public class Order
{
    public string column { get; set; }
    public string dir { get; set; }
}
```

### *DataTableResult* class
Response data should be struct like below

```c#
[JsonObject(Title = "result")]
public class DataTableResult<T>
{
    [JsonProperty("draw")]
    public int Draw { get; set; }

    [JsonProperty("recordsTotal")]
    public int RecordsTotal { get; set; }

    [JsonProperty("recordsFiltered")]
    public int RecordsFiltered { get; set; }

    [JsonProperty("data")]
    public List<T> Data { get; set; }

    public DataTableResult(int Draw, int RecordsTotal, int RecordsFiltered, List<T> Data)
    {
        if (Data == null)
        {
            Data = new List<T>();
        }

        this.Draw = Draw;
        this.RecordsTotal = RecordsTotal;
        this.RecordsFiltered = RecordsFiltered;
        this.Data = Data;
    }
}
```
### Handle request from client

```c#
public class UserController : Controller
{
    [HttpPost]
    public ContentResult GetDataTable(DataTableRequest querry)
    {
        // Query data
        var Data = db.Users;
        
        // Search
        string searchkey = querry.search?.value ?? "";
        var searchData = Data.Where(d => d.Name.Contains(searchkey) || d.Code.Contains(searchkey));
        
        // TODO: short

        // paging
        var pagingData = searchData.ToList().Skip(querry.start).Take(querry.length);

        var dataResult = JsonConvert.SerializeObject(queryData.ToList());

        var returnResult = new DataTableResult<CategoryDto>(querry.draw, Data.Count(), searchData.Count(), dataResult);

        return Content(returnResult, "application/json");
    }
}


[JsonObject(Title = "user")]
public class User
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("first_name")]
    public string FirstName { get; set; }

    [JsonProperty("last_name")]
    public string LastName { get; set; }
}
```
