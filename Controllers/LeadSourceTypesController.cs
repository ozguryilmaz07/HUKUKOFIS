// -----------------------------------------------------------------------
// <copyright file="LeadSourceTypesController.cs" company="Nodine Legal, LLC">
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
    using System.Web.Mvc;
    using AutoMapper;
    using System.Collections.Generic;
    using System.Data;

    [HandleError(View = "Errors/Index", Order = 10)]
    public class LeadSourceTypesController : BaseController
    {
        [Authorize(Roles = "Login, User")]
        public ActionResult Index()
        {
            List<ViewModels.Leads.LeadSourceTypeViewModel> vmList = new List<ViewModels.Leads.LeadSourceTypeViewModel>();

            using (IDbConnection conn = Data.Database.Instance.GetConnection())
            {
                Data.Leads.LeadSourceType.List(conn, false).ForEach(x =>
                {
                    vmList.Add(Mapper.Map<ViewModels.Leads.LeadSourceTypeViewModel>(x));
                });
            }

            return View(vmList);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Details(int id)
        {
            ViewModels.Leads.LeadSourceTypeViewModel viewModel;
            Common.Models.Leads.LeadSourceType model;

            using (IDbConnection conn = Data.Database.Instance.GetConnection())
            {
                model = Data.Leads.LeadSourceType.Get(id, conn, false);

                viewModel = Mapper.Map<ViewModels.Leads.LeadSourceTypeViewModel>(model);

                PopulateCoreDetails(viewModel, conn);
            }

            return View(viewModel);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(int id)
        {
            ViewModels.Leads.LeadSourceTypeViewModel viewModel;
            Common.Models.Leads.LeadSourceType model;

            model = Data.Leads.LeadSourceType.Get(id);

            viewModel = Mapper.Map<ViewModels.Leads.LeadSourceTypeViewModel>(model);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(int id, ViewModels.Leads.LeadSourceTypeViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Leads.LeadSourceType model;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    model = Mapper.Map<Common.Models.Leads.LeadSourceType>(viewModel);

                    model = Data.Leads.LeadSourceType.Edit(trans, model, currentUser);

                    trans.Commit();

                    return RedirectToAction("Index");
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
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Create(ViewModels.Leads.LeadSourceTypeViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Leads.LeadSourceType model;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    model = Mapper.Map<Common.Models.Leads.LeadSourceType>(viewModel);

                    model = Data.Leads.LeadSourceType.Create(trans, model, currentUser);

                    trans.Commit();

                    return RedirectToAction("Index");
                }
                catch
                {
                    trans.Rollback();
                    return Create();
                }
            }
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Delete(int id)
        {
            ViewModels.Leads.LeadSourceTypeViewModel viewModel;
            Common.Models.Leads.LeadSourceType model;

            model = Data.Leads.LeadSourceType.Get(id);

            viewModel = Mapper.Map<ViewModels.Leads.LeadSourceTypeViewModel>(model);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Delete(int id, ViewModels.Leads.LeadSourceTypeViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Leads.LeadSourceType model;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    model = Mapper.Map<Common.Models.Leads.LeadSourceType>(viewModel);

                    model = Data.Leads.LeadSourceType.Disable(trans, model, currentUser);

                    trans.Commit();

                    return RedirectToAction("Index");
                }
                catch
                {
                    trans.Rollback();
                    return Delete(id);
                }
            }
        }
    }
}
