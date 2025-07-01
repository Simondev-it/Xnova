namespace Xnova.API.RequestModel
{
    public class WeatherResponse
    {
        public WeatherMain main { get; set; }
        public List<WeatherDescription> weather { get; set; }
        public string name { get; set; } // tên khu vực
    }

    public class WeatherMain
    {
        public double temp { get; set; }
        public double humidity { get; set; }
    }

    public class WeatherDescription
    {
        public string main { get; set; } // ví dụ: Rain, Clear
        public string description { get; set; }
    }

}
