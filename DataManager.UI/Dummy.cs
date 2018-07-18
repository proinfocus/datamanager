using DataManager.Core;

namespace DataManager.UI
{
    public class Dummy
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
