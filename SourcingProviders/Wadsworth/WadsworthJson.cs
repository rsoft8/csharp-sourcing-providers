using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcSoftware.SourcingProviders.Wadsworth
{
    public class AjaxPageState
    {
        public string theme { get; set; }
        public string theme_token { get; set; }
        public string jquery_version { get; set; }
    }

    public class Colorbox
    {
        public string opacity { get; set; }
        public string current { get; set; }
        public string previous { get; set; }
        public string next { get; set; }
        public string close { get; set; }
        public string maxWidth { get; set; }
        public string maxHeight { get; set; }
        public bool @fixed { get; set; }
        public bool mobiledetect { get; set; }
        public string mobiledevicewidth { get; set; }
    }

    public class FieldLabTypeElapTid
    {
        public bool required { get; set; }
    }

    public class FieldCategoryNameElapTid
    {
        public bool required { get; set; }
    }

    public class FieldAnalyteNameElapTid
    {
        public bool required { get; set; }
    }

    public class FieldLabIdElapValue
    {
        public bool required { get; set; }
    }

    public class FieldLabNameElapValue
    {
        public bool required { get; set; }
    }

    public class FieldCityElapTid
    {
        public bool required { get; set; }
    }

    public class FieldZipCodeElapValue
    {
        public bool required { get; set; }
    }

    public class FieldStateElapTid
    {
        public bool required { get; set; }
    }

    public class FieldCountyElapTid
    {
        public bool required { get; set; }
    }

    public class FieldCountryElapTid
    {
        public bool required { get; set; }
    }

    public class Filters
    {
        public FieldLabTypeElapTid field_lab_type_elap_tid { get; set; }
        public FieldCategoryNameElapTid field_category_name_elap_tid { get; set; }
        public FieldAnalyteNameElapTid field_analyte_name_elap_tid { get; set; }
        public FieldLabIdElapValue field_lab_id_elap_value { get; set; }
        public FieldLabNameElapValue field_lab_name_elap_value { get; set; }
        public FieldCityElapTid field_city_elap_tid { get; set; }
        public FieldZipCodeElapValue field_zip_code_elap_value { get; set; }
        public FieldStateElapTid field_state_elap_tid { get; set; }
        public FieldCountyElapTid field_county_elap_tid { get; set; }
        public FieldCountryElapTid field_country_elap_tid { get; set; }
    }

    public class Elap2
    {
        public Filters filters { get; set; }
    }

    public class Displays
    {
        public Elap2 elap { get; set; }
    }

    public class Elap
    {
        public Displays displays { get; set; }
    }

    public class Views
    {
        public Elap elap { get; set; }
    }

    public class BetterExposedFilters
    {
        public bool datepicker { get; set; }
        public bool slider { get; set; }
        public List<object> settings { get; set; }
        public Views views { get; set; }
    }

    public class EditFieldLabTypeElapTid
    {
        public string noneLabel { get; set; }
        public string noneValue { get; set; }
        public List<object> labels { get; set; }
    }

    public class EditFieldCategoryNameElapTid
    {
        public string noneLabel { get; set; }
        public string noneValue { get; set; }
        public List<object> labels { get; set; }
    }

    public class EditFieldAnalyteNameElapTid
    {
        public string noneLabel { get; set; }
        public string noneValue { get; set; }
        public List<object> labels { get; set; }
    }

    public class EditFieldStateElapTid
    {
        public string noneLabel { get; set; }
        public string noneValue { get; set; }
        public List<object> labels { get; set; }
    }

    public class EditFieldCountyElapTid
    {
        public string noneLabel { get; set; }
        public string noneValue { get; set; }
        public List<object> labels { get; set; }
    }

    public class EditFieldCountryElapTid
    {
        public string noneLabel { get; set; }
        public string noneValue { get; set; }
        public List<object> labels { get; set; }
    }

    public class Cshs
    {
        [JsonProperty("edit-field-lab-type-elap-tid")]
        public EditFieldLabTypeElapTid FieldLabTypeElapTid { get; set; }

        [JsonProperty("edit-field-category-name-elap-tid")]
        public EditFieldCategoryNameElapTid FieldCategoryNameElapTid { get; set; }

        [JsonProperty("edit-field-analyte-name-elap-tid")]
        public EditFieldAnalyteNameElapTid FieldAnalyteNameElapTid { get; set; }

        [JsonProperty("edit-field-state-elap-tid")]
        public EditFieldStateElapTid FieldStateElapTid { get; set; }
        
    [JsonProperty("edit-field-county-elap-tid")]
public EditFieldCountyElapTid FieldcountyElapTid { get; set; }
        
    [JsonProperty("edit-field-country-elap-tid")]
public EditFieldCountryElapTid FieldCountryElapTid { get; set; }
    }

    public class UrlIsAjaxTrusted
    {
        [JsonProperty("/")]
        public bool Slash { get; set; }
    }

    public class ViewsDomId28dc102180cd78b1e337ab98c9be9095
    {
        public string view_name { get; set; }
        public string view_display_id { get; set; }
        public string view_args { get; set; }
        public string view_path { get; set; }
        public object view_base_path { get; set; }
        public string view_dom_id { get; set; }
        public int pager_element { get; set; }
    }

    public class AjaxViews
    {
        [JsonProperty("views_dom_id:28dc102180cd78b1e337ab98c9be9095")]
        public ViewsDomId28dc102180cd78b1e337ab98c9be9095 ViewsDomId { get; set; }
    }

    public class Views2
    {
        public string ajax_path { get; set; }
        public AjaxViews ajaxViews { get; set; }
    }

    public class Settings
    {
        public string basePath { get; set; }
        public string pathPrefix { get; set; }
        public AjaxPageState ajaxPageState { get; set; }
        public Colorbox colorbox { get; set; }
        public BetterExposedFilters better_exposed_filters { get; set; }
        public Cshs cshs { get; set; }
        public UrlIsAjaxTrusted urlIsAjaxTrusted { get; set; }
        public Views2 views { get; set; }
    }

    public class RootObject
    {
        public string command { get; set; }
        public Settings settings { get; set; }
        public bool merge { get; set; }
        public string method { get; set; }
        public string selector { get; set; }
        public string data { get; set; }
    }

}
