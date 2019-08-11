using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Website.Models;

namespace Website.Controllers
{
    [Authorize(Roles = "user")]
    public class DetailScController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index(int id)
        {
            var dbh = DbContext.Get();
            var data = dbh.Db.Queryable<SkinCareClass>().InSingle(id);
            detailScModel model = new detailScModel();
            model.data = data;

            var pcnt = SettingModel.getInstance().skincare_apc_p_day;

            var appointmentData = dbh.Db.Queryable<SkinCareOrder>()
                .Where(ii => ii.ClassId == id && ii.reserveDate >= DateTime.Now.Date && ii.reserveDate <= DateTime.Now.Date.AddDays(9))
                .GroupBy(ii => ii.reserveDate)
                .Select(ii => new { ii.reserveDate, Cnt = pcnt - SqlSugar.SqlFunc.AggregateCount(ii.id) }).ToList();

            var tmpobj = new JObject();
            var Vobj = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(appointmentData));
            var thisDay = DateTime.Now.Date;
            for (int i = 0; i < 9; i++)
            {
                tmpobj.Add(thisDay.ToString("yyyy-MM-dd"), pcnt);
                thisDay = thisDay.AddDays(1);
            }
            foreach (var item in appointmentData)
            {
                tmpobj[item.reserveDate.ToString("yyyy-MM-dd")] = item.Cnt;
            }

            foreach (var item in tmpobj)
            {
                model.dayinfo.Add(new detailScModel.AppointmentInfo() { date = DateTime.Parse(item.Key), remain = int.Parse(item.Value.ToString()) });
            }
            return View(model);
        }
        public IActionResult Save()
        {
            return View();
        }
    }
}