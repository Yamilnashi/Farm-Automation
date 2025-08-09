using Newtonsoft.Json;
using System.Reflection;

namespace FarmToTableWebApp.Data
{
    public class Mapper
    {
        public string GetTableDataJson<T>(IEnumerable<T> data)
        {
            return JsonConvert.SerializeObject(new { data });
        }

        private IEnumerable<DataTableColumnAttribute> GetColumns(Type type, int? extraCols = null, bool? camelCaseOutput = false)
        {
            List<DataTableColumnAttribute> list = new List<DataTableColumnAttribute>();
            PropertyInfo[] properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                DataTableColumnAttribute? customAttribute = propertyInfo.GetCustomAttribute<DataTableColumnAttribute>();
                if (customAttribute != null)
                {
                    if (camelCaseOutput.GetValueOrDefault())
                    {
                        customAttribute.Name = char.ToLowerInvariant(propertyInfo.Name[0]) + propertyInfo.Name.Substring(1);
                    }
                    else
                    {
                        customAttribute.Name = propertyInfo.Name;
                    }

                    list.Add(customAttribute);
                }
            }

            IEnumerable<int> enumerable = from x in list.Where(x => x.ColIndexs != null)
                                                        .SelectMany((DataTableColumnAttribute x) => x.ColIndexs!)
                                          group x by x into x
                                          where x.Count() > 1
                                          select x.Key;
            if (enumerable.Count() > 0)
            {
                throw new ArgumentException("Duplicate columns target: " + string.Join(",", enumerable));
            }

            int num = list
                .Where(x => x.ColIndexs != null)
                .SelectMany((DataTableColumnAttribute x) => x.ColIndexs!)
                .ToList()
                .Max() + extraCols.GetValueOrDefault();
            List<DataTableColumnAttribute> list2 = new List<DataTableColumnAttribute>();
            int i;
            for (i = 0; i <= num; i++)
            {
                DataTableColumnAttribute? item = list
                    .Where(x => x.ColIndexs != null)
                    .FirstOrDefault((DataTableColumnAttribute x) => x.ColIndexs!.Contains(i));
                if (item != null)
                {
                    list2.Add(item);
                }
            }

            for (int k = 0; k <= num; k++)
            {
                if (list2[k] != null)
                {
                    list2[k].ColIndexs = null;
                    continue;
                }

                list2[k] = new DataTableColumnAttribute
                {
                    DefaultContent = string.Empty,
                    Name = string.Empty
                };
            }

            return list2;
        }

        public string GetColumnsJson(Type type, int? extraCols = null, bool? camelCaseOuput = false)
        {
            return JsonConvert.SerializeObject(GetColumns(type, extraCols, camelCaseOuput));
        }
    }
}
