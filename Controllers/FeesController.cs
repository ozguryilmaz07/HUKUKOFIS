﻿// -----------------------------------------------------------------------
// <copyright file="FeesController.cs" company="Nodine Legal, LLC">
// Licensed to Nodine Legal, LLC under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  Nodine Legal, LLC licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
// </copyright>
// -----------------------------------------------------------------------

namespace OpenLawOffice.Web.Controllers
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using AutoMapper;
    using System.Data;

    [HandleError(View = "Errors/Index", Order = 10)]

    [Authorize]
    public class FeesController : Controller
    {
        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(Guid id)
        {
            Common.Models.Billing.Fee model;
            Common.Models.Matters.Matter matter;
            ViewModels.Billing.FeeViewModel viewModel;

            using (IDbConnection conn = Data.Database.Instance.GetConnection())
            {
                model = Data.Billing.Fee.Get(id, conn, false);
                matter = Data.Billing.Fee.GetMatter(id, conn, false);
            }

            viewModel = Mapper.Map<ViewModels.Billing.FeeViewModel>(model);

            ViewBag.Matter = matter;
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(Guid id, ViewModels.Billing.FeeViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Matters.Matter matter;
            Common.Models.Billing.Fee model;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(User.Identity.Name);

                    matter = Data.Billing.Fee.GetMatter(id);

                    model = Mapper.Map<Common.Models.Billing.Fee>(viewModel);

                    model = Data.Billing.Fee.Edit(model, currentUser);

                    trans.Commit();

                    return RedirectToAction("Details", "Matters", new { Id = matter.Id });
                }
                catch
                {
                    trans.Rollback();
                    return Edit(id);
                }
            }
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Create()
        {
            Common.Models.Matters.Matter matter = null;

            using (IDbConnection conn = Data.Database.Instance.GetConnection())
            {
                matter = Data.Matters.Matter.Get(Guid.Parse(Request["MatterId"]));
            }

            ViewBag.Matter = Mapper.Map<ViewModels.Matters.MatterViewModel>(matter);

            return View(new ViewModels.Billing.FeeViewModel()
            {
                Incurred = DateTime.Now
            });
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Create(ViewModels.Billing.FeeViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Billing.Fee model;
            Guid matterid;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(User.Identity.Name);

                    model = Mapper.Map<Common.Models.Billing.Fee>(viewModel);

                    model = Data.Billing.Fee.Create(model, currentUser);

                    matterid = Guid.Parse(Request["MatterId"]);

                    Data.Billing.Fee.RelateMatter(model, matterid, currentUser);

                    trans.Commit();

                    return RedirectToAction("Details", "Matters", new { Id = matterid });
                }
                catch
                {
                    trans.Rollback();
                    return Create();
                }
            }
        }
    }
}
