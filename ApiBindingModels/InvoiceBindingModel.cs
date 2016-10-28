using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiBindingModels {
    public class InvoiceInitBindingModel
    {
        public List<InvoiceIconLookupBindingModel> Icons { get; set; }
        public List<InvoiceGradeLookupBindingModel> Grades { get; set; }
        public List<InvoiceTeacherLookupBindingModel> Teachers { get; set; }
        public string SchoolName { get; set; }
    }

    public class InvoiceGradeLookupBindingModel {
        public string schcode { get; set; }
        public string grade { get; set; }
        public string id { get; set; }
    }

    public class InvoiceSchoolNameBindingModel
    {
        public string schoolname { get; set; }
    }

    public class InvoiceIconLookupBindingModel
    {
        public string id { get; set; }
        public string cvalue { get; set; }
        public string isortorder { get; set; }
        public string caption { get; set; }
        public string ivalue { get; set; }
        public string csortorder { get; set; }
    }

    public class InvoiceTeacherLookupBindingModel{
        public string schcode { get; set; }
        public string invno { get; set; }
        public string teacher { get; set; }
        public string id { get; set; }
    }
}
