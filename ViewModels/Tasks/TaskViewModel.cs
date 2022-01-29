﻿// -----------------------------------------------------------------------
// <copyright file="TaskViewModel.cs" company="Nodine Legal, LLC">
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

namespace OpenLawOffice.Web.ViewModels.Tasks
{
    using System;
    using AutoMapper;
    using OpenLawOffice.Common.Models;
    using System.Collections.Generic;

    [MapMe]
    public class TaskViewModel : CoreViewModel
    {
        public long? Id { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public DateTime? ProjectedStart { get; set; }

        public DateTime? DueDate { get; set; }

        public DateTime? ProjectedEnd { get; set; }

        public DateTime? ActualEnd { get; set; }

        public TaskViewModel Parent { get; set; }

        public bool IsGroupingTask { get; set; }

        public TaskViewModel SequentialPredecessor { get; set; }

        public bool Active { get; set; }

        public List<ViewModels.Notes.NoteViewModel> Notes { get; set; }

        public List<ViewModels.Timing.TimeViewModel> Times { get; set; }

        public void BuildMappings()
        {
            Mapper.CreateMap<Common.Models.Tasks.Task, TaskViewModel>()
                .ForMember(dst => dst.IsStub, opt => opt.UseValue(false))
                .ForMember(dst => dst.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(dst => dst.Modified, opt => opt.MapFrom(src => src.Modified))
                .ForMember(dst => dst.Disabled, opt => opt.MapFrom(src => src.Disabled))
                .ForMember(dst => dst.CreatedBy, opt => opt.ResolveUsing(db =>
                {
                    return new ViewModels.Account.UsersViewModel()
                    {
                        PId = db.CreatedBy.PId,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.ModifiedBy, opt => opt.ResolveUsing(db =>
                {
                    return new ViewModels.Account.UsersViewModel()
                    {
                        PId = db.ModifiedBy.PId,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.DisabledBy, opt => opt.ResolveUsing(db =>
                {
                    if (db.DisabledBy == null || !db.DisabledBy.PId.HasValue) return null;
                    return new ViewModels.Account.UsersViewModel()
                    {
                        PId = db.DisabledBy.PId.Value,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Parent, opt => opt.ResolveUsing(db =>
                {
                    if (db.Parent == null || !db.Parent.Id.HasValue)
                        return null;
                    return new TaskViewModel()
                    {
                        Id = db.Parent.Id.Value,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Type, opt => opt.Ignore())
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.ProjectedStart, opt => opt.MapFrom(src => src.ProjectedStart))
                .ForMember(dst => dst.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dst => dst.ProjectedEnd, opt => opt.MapFrom(src => src.ProjectedEnd))
                .ForMember(dst => dst.ActualEnd, opt => opt.MapFrom(src => src.ActualEnd))
                .ForMember(dst => dst.IsGroupingTask, opt => opt.MapFrom(src => src.IsGroupingTask))
                .ForMember(dst => dst.SequentialPredecessor, opt => opt.ResolveUsing(db =>
                {
                    if (db.SequentialPredecessor == null || !db.SequentialPredecessor.Id.HasValue)
                        return null;
                    return new TaskViewModel()
                    {
                        Id = db.SequentialPredecessor.Id.Value,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.Active, opt => opt.MapFrom(src => src.Active))
                .ForMember(dst => dst.Notes, opt => opt.Ignore())
                .ForMember(dst => dst.Times, opt => opt.Ignore());

            Mapper.CreateMap<TaskViewModel, Common.Models.Tasks.Task>()
                .ForMember(dst => dst.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(dst => dst.Modified, opt => opt.MapFrom(src => src.Modified))
                .ForMember(dst => dst.Disabled, opt => opt.MapFrom(src => src.Disabled))
                .ForMember(dst => dst.CreatedBy, opt => opt.ResolveUsing(x =>
                {
                    if (x.CreatedBy == null || !x.CreatedBy.PId.HasValue)
                        return null;
                    return new ViewModels.Account.UsersViewModel()
                    {
                        PId = x.CreatedBy.PId
                    };
                }))
                .ForMember(dst => dst.ModifiedBy, opt => opt.ResolveUsing(x =>
                {
                    if (x.CreatedBy == null || !x.CreatedBy.PId.HasValue)
                        return null;
                    return new ViewModels.Account.UsersViewModel()
                    {
                        PId = x.ModifiedBy.PId
                    };
                }))
                .ForMember(dst => dst.DisabledBy, opt => opt.ResolveUsing(x =>
                {
                    if (x.DisabledBy == null || !x.DisabledBy.PId.HasValue)
                        return null;
                    return new ViewModels.Account.UsersViewModel()
                    {
                        PId = x.DisabledBy.PId.Value
                    };
                }))
                .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dst => dst.Parent, opt => opt.ResolveUsing(model =>
                {
                    if (model.Parent == null || !model.Parent.Id.HasValue)
                        return null;
                    return new Common.Models.Tasks.Task()
                    {
                        Id = model.Parent.Id,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dst => dst.ProjectedStart, opt => opt.MapFrom(src => src.ProjectedStart))
                .ForMember(dst => dst.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dst => dst.ProjectedEnd, opt => opt.MapFrom(src => src.ProjectedEnd))
                .ForMember(dst => dst.ActualEnd, opt => opt.MapFrom(src => src.ActualEnd))
                .ForMember(dst => dst.IsGroupingTask, opt => opt.MapFrom(src => src.IsGroupingTask))
                .ForMember(dst => dst.SequentialPredecessor, opt => opt.ResolveUsing(model =>
                {
                    if (model.SequentialPredecessor == null || !model.SequentialPredecessor.Id.HasValue)
                        return null;
                    return new Common.Models.Tasks.Task()
                    {
                        Id = model.SequentialPredecessor.Id,
                        IsStub = true
                    };
                }))
                .ForMember(dst => dst.Active, opt => opt.MapFrom(src => src.Active));
        }
    }
}