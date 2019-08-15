using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using App.Data;
using MakC.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Website.Models;

namespace Website.Controllers
{
    [Authorize(Roles = "user")]
    public class CardsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult list()
        {
            var dbh = DbContext.Get();
            List<Card> data = dbh.Db.Queryable<Card>().Where(ii=>!ii.disabled && ii.canBuy).ToList();
            
            return View(data);
        }
        public IActionResult buy(int id)
        {
            Card card = DbContext.Get().GetEntityDB<Card>().GetById(id);
            return View(card);
        }
        [HttpPost("/cards/buy")]
        public IActionResult buy_save(int id)
        {
            var userid = User.FindFirst(ClaimTypes.Sid).Value.AsInt();
            var dbh = DbContext.Get();
            Card card = dbh.GetEntityDB<Card>().GetById(id);

            UserCard userCard = new UserCard();
            userCard.create_at = DateTime.Now;
            userCard.userid = userid;
            userCard.cardsId = card.id;
            userCard.cardname = card.name;
            userCard.cardtype = card.type;
            userCard.cardexpiryDate = DateTime.Now.AddDays(card.expiry);
            userCard.usedCnt = 0;
            userCard.canUseCnt = card.canUseCount;

            int optcnt =dbh.Db.Insertable(userCard).ExecuteCommand();
            if (optcnt == 0)
            {
                return ShowErrorPage("购买失败");
            }
            return View("save",card);
        }
        private IActionResult ShowErrorPage(string msg, string description = "")
        {
            ErrorViewModel erv = new ErrorViewModel();
            erv.Msg = msg;
            erv.Description = description;
            erv.Url = HttpContext.Request.Path;
            //erv.Referer = HttpContext.Request.Headers["Referer"];
            return View("Error", erv);
        }
    }
}