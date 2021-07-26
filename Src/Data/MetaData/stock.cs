using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    [MetadataType(typeof(stockMetaData))]
    public partial class stock
    {

    }

    public class stockMetaData
    {
        [Range(0,int.MaxValue)]
        public int quantity;
    }
}
