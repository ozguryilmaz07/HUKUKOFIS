﻿// -----------------------------------------------------------------------
// <copyright file="MatterTypesController.cs" company="Nodine Legal, LLC">
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
    public class MatterTypesController : BaseController
    {
        [Authorize(Roles = "Login, User")]
        public ActionResult Index()
        {
            List<ViewModels.Matters.MatterTypeViewModel> vmList = new List<ViewModels.Matters.MatterTypeViewModel>();

            using (IDbConnection conn = Data.Database.Instance.GetConnection())
            {
                Data.Matters.MatterType.List(conn, false).ForEach(x =>
                {
                    vmList.Add(Mapper.Map<ViewModels.Matters.MatterTypeViewModel>(x));
                });
            }

            return View(vmList);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Details(int id)
        {
            ViewModels.Matters.MatterTypeViewModel viewModel;
            Common.Models.Matters.MatterType model;

            using (IDbConnection conn = Data.Database.Instance.GetConnection())
            {
                model = Data.Matters.MatterType.Get(id, conn, false);

                viewModel = Mapper.Map<ViewModels.Matters.MatterTypeViewModel>(model);

                PopulateCoreDetails(viewModel, conn);
            }

            return View(viewModel);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(int id)
        {
            ViewModels.Matters.MatterTypeViewModel viewModel;
            Common.Models.Matters.MatterType model;

            model = Data.Matters.MatterType.Get(id);

            viewModel = Mapper.Map<ViewModels.Matters.MatterTypeViewModel>(model);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(int id, ViewModels.Matters.MatterTypeViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Matters.MatterType model;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    model = Mapper.Map<Common.Models.Matters.MatterType>(viewModel);

                    model = Data.Matters.MatterType.Edit(trans, model, currentUser);

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
        public ActionResult Create(ViewModels.Matters.MatterTypeViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Matters.MatterType model;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    model = Mapper.Map<Common.Models.Matters.MatterType>(viewModel);

                    model = Data.Matters.MatterType.Create(trans, model, currentUser);

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
            ViewModels.Matters.MatterTypeViewModel viewModel;
            Common.Models.Matters.MatterType model;

            model = Data.Matters.MatterType.Get(id);

            viewModel = Mapper.Map<ViewModels.Matters.MatterTypeViewModel>(model);

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Delete(int id, ViewModels.Matters.MatterTypeViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Matters.MatterType model;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    model = Mapper.Map<Common.Models.Matters.MatterType>(viewModel);

                    model = Data.Matters.MatterType.Disable(trans, model, currentUser);

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
