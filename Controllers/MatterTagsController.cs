﻿// -----------------------------------------------------------------------
// <copyright file="MatterTagsController.cs" company="Nodine Legal, LLC">
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
    using System.Web.Mvc;
    using AutoMapper;
    using System.Data;

    // NOT MAINTAINED
    [HandleError(View = "Errors/Index", Order = 10)]
    public class MatterTagsController : BaseController
    {
        [Authorize(Roles = "Login, User")]
        public ActionResult Details(Guid id)
        {
            ViewModels.Matters.MatterTagViewModel viewModel;
            Common.Models.Matters.MatterTag model;

            using (IDbConnection conn = Data.Database.Instance.GetConnection())
            {
                model = Data.Matters.MatterTag.Get(id, conn, false);
                model.Matter = Data.Matters.Matter.Get(model.Matter.Id.Value, conn, false);

                viewModel = Mapper.Map<ViewModels.Matters.MatterTagViewModel>(model);
                viewModel.Matter = Mapper.Map<ViewModels.Matters.MatterViewModel>(model.Matter);

                PopulateCoreDetails(viewModel, conn);
            }

            ViewData["MatterId"] = model.Matter.Id.Value;
            ViewData["Matter"] = model.Matter.Title;

            return View(viewModel);
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Create(Guid id)
        {
            Common.Models.Matters.Matter matter;

            matter = Data.Matters.Matter.Get(id);

            ViewData["MatterId"] = matter.Id.Value;
            ViewData["Matter"] = matter.Title;

            return View(new ViewModels.Matters.MatterTagViewModel()
            {
                Matter = Mapper.Map<ViewModels.Matters.MatterViewModel>(matter)
            });
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Create(ViewModels.Matters.MatterTagViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Matters.MatterTag model;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    // Need to overwrite the ID received as it pertains to the MatterId
                    viewModel.Id = Guid.NewGuid();
                    model = Mapper.Map<Common.Models.Matters.MatterTag>(viewModel);

                    model.Matter = new Common.Models.Matters.Matter()
                    {
                        Id = Guid.Parse(RouteData.Values["Id"].ToString())
                    };

                    model.TagCategory = Mapper.Map<Common.Models.Tagging.TagCategory>(viewModel.TagCategory);

                    model = Data.Matters.MatterTag.Create(trans, model, currentUser);

                    trans.Commit();

                    return RedirectToAction("Tags", "Matters", new { Id = model.Matter.Id.Value.ToString() });
                }
                catch
                {
                    trans.Rollback();
                    return Create(viewModel.Matter.Id.Value);
                }
            }
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(Guid id)
        {
            ViewModels.Matters.MatterTagViewModel viewModel;
            Common.Models.Matters.MatterTag model;

            using (IDbConnection conn = Data.Database.Instance.GetConnection())
            {
                model = Data.Matters.MatterTag.Get(id, conn, false);
                model.Matter = Data.Matters.Matter.Get(model.Matter.Id.Value, conn, false);

                viewModel = Mapper.Map<ViewModels.Matters.MatterTagViewModel>(model);
                viewModel.Matter = Mapper.Map<ViewModels.Matters.MatterViewModel>(model.Matter);

                PopulateCoreDetails(viewModel, conn);
            }

            ViewData["MatterId"] = model.Matter.Id.Value;
            ViewData["Matter"] = model.Matter.Title;

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Edit(Guid id, ViewModels.Matters.MatterTagViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Matters.MatterTag model;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    model = Mapper.Map<Common.Models.Matters.MatterTag>(viewModel);
                    model.TagCategory = Mapper.Map<Common.Models.Tagging.TagCategory>(viewModel.TagCategory);
                    model.Matter = Data.Matters.MatterTag.Get(trans, id).Matter;

                    model = Data.Matters.MatterTag.Edit(trans, model, currentUser);

                    trans.Commit();

                    return RedirectToAction("Tags", "Matters", new { Id = model.Matter.Id.Value.ToString() });
                }
                catch
                {
                    trans.Rollback();
                    return Edit(id);
                }
            }
        }

        [Authorize(Roles = "Login, User")]
        public ActionResult Delete(Guid id)
        {
            return Details(id);
        }

        [HttpPost]
        [Authorize(Roles = "Login, User")]
        public ActionResult Delete(Guid id, ViewModels.Matters.MatterTagViewModel viewModel)
        {
            Common.Models.Account.Users currentUser;
            Common.Models.Matters.MatterTag model;

            using (Data.Transaction trans = Data.Transaction.Create(true))
            {
                try
                {
                    currentUser = Data.Account.Users.Get(trans, User.Identity.Name);

                    model = Mapper.Map<Common.Models.Matters.MatterTag>(viewModel);

                    model = Data.Matters.MatterTag.Disable(trans, model, currentUser);

                    trans.Commit();

                    return RedirectToAction("Tags", "Matters", new { Id = model.Matter.Id.Value.ToString() });
                }
                catch
                {
                    trans.Rollback();
                    return Edit(id);
                }
            }
        }
    }
}