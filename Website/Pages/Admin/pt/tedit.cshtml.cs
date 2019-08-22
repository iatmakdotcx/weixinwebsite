using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Data;
using App.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Website.Pages.Admin
{
    [AdminAuthorize(Roles = "Admin")]
    public class teditModel : PageModel
    {
        public App.Data.PTTemplate data = null;
        public string teacherName = null;
        public void OnGet(int id = 0)
        {
            if (id > 0)
            {
                var dbh = DbContext.Get();
                data = dbh.GetEntityDB<App.Data.PTTemplate>().GetById(id);
                teacherName = dbh.Db.Queryable<App.Data.UserInfo>().Select(ii => new { ii.id, ii.nickname }).First(ii => ii.id == data.userId)?.nickname;
            }
        }
    }
}