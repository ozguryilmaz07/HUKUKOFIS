﻿// -----------------------------------------------------------------------
// <copyright file="BillingRatesController.cs" company="Nodine Legal, LLC">
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
    using System.Collections.Generic;
    using System.Web.Mvc;
    using AutoMapper;
    using System.Data;

    [HandleError(View = "Errors/Index", Order = 10)]
    public class BillingRatesController : BaseController
    {
        [Authorize(Roles = "Login, User")]
        public ActionResult Index()
        {
            List<ViewModels.Billing.BillingRateViewModel> rates = new List<ViewModels.Billing.BillingRateViewModel>();

            using (IDbConnection conn = Data.Database.Instance.GetConnection())
            {
                Data.Billing.BillingRate.List(conn, false).ForEach(x =>
                {
                    rates.Add(Mapper.Map<ViewModels.Billing.BillingRateViewModel>(x));
                });
            }

            return View(rates);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Login, User")]
        [HttpPost]
        public ActionResult Create(ViewModels.Billing.BillingRateViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Billing.BillingRate model;

            model = Mapper.Map<Common.Models.Billing.BillingRate>(viewModel);

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    Data.Billing.BillingRate.Create(trans, model, currentUser);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(int id)
        {
            ViewModels.Billing.BillingRateViewModel vm;

            using (IDbConnection conn = Data.Database.Instance.GetConnection())
            {
                vm = Mapper.Map<ViewModels.Billing.BillingRateViewModel>(
                    Data.Billing.BillingRate.Get(id, conn, false));
            }

            return View(vm);
        }

        [Authorize(Roles = "Login, User")]
        [HttpPost]
        public ActionResult Edit(int id, ViewModels.Billing.BillingRateViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Billing.BillingRate model;

            model = Mapper.Map<Common.Models.Billing.BillingRate>(viewModel);

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    Data.Billing.BillingRate.Edit(trans, model, currentUser);

                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }

            return RedirectToAction("Index");
        }
    }
}