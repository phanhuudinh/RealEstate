Fill data cho Datatable từ server side
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
        // data trả về từ server, nên có cấu trúc bao gồm các file đã định nghĩa
        // như trên
        console.log(JSON.stringify(data));
        return JSON.stringify(data);
    }
  },
})
```
### POST data
Lúc thao tác trên table như chuyển trang, search, sắp xếp thì DataTable sẽ tạo requet tương ứng gửi lên server, Post data có cấu trúc như sau
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
- `draw` biến dùng để nhận biết và vẽ lại table
- `start` vị trí bắt đầu lấy dữ liệu
- `length` số dòng cần lấy
- `search` object search gồm `value` là từ khóa để search và `regex` (*true* | *false*) để xác định có xử dụng regex hay không
- `order` mãng các object oder của từng column, mỗi object gồm `column` là index của column tương ứng trên table và `dir` là kiểu sort (*"asc"* | *"desc"*)
- `columns` mãng chứa option và thông tin khác của các column


## Server side
### *DataTableRequest* class
Dựa vào cấu trúc data post lên từ client, ta tạo class tương ứng để xử lý

```c#
public class DataTableRequest
{
    public int draw { get; set; }
    public int start { get; set; }
    public int length { get; set; }
    // Chưa dùng nên định nghĩa là object
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
Data trả về cũng phải theo một cấu trúc để DataTable phía client có thể hiểu, class `DataTableResult` có cấu trúc tương ứng như sau

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
### Xửa lý request

```c#
public class UserController : Controller
{
    [HttpPost]
    public ContentResult GetDataTable(DataTableRequest querry)
    {
        // Data query từ Users entity
        var Data = db.Users;
        
        // Search
        string searchkey = querry.search?.value ?? "";
        var searchData = Data.Where(d => d.Name.Contains(searchkey) || d.Code.Contains(searchkey));
        
        // TODO: Sắp xếp

        // Phân trang
        var pagingData = searchData.ToList().Skip(querry.start).Take(querry.length);

        var dataResult = JsonConvert.SerializeObject(queryData.ToList());

        var returnResult = new DataTableResult<CategoryDto>(querry.draw, Data.Count(), searchData.Count(), dataResult);

        return Content(returnResult, "application/json");
    }
}

// Class User nên định nghĩa các json property để Serialize sang json object lúc trả về
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