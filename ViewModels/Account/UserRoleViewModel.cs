// -----------------------------------------------------------------------
// <copyright file="UserRoleViewModel.cs" company="Nodine Legal, LLC">
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

namespace OpenLawOffice.Web.ViewModels.Account
{
    using System;
    using System.Collections.Generic;
    using AutoMapper;
    using OpenLawOffice.Common.Models;

    [MapMe]
    public class UserRoleViewModel : ViewModelBase
    {
        public string Username { get; set; }

        public string Rolename { get; set; }

        public string ApplicationName { get; set; }

        public void BuildMappings()
        {
            Mapper.CreateMap<Common.Models.Account.UserRole, UserRoleViewModel>()
                .ForMember(dst => dst.IsStub, opt => opt.UseValue(false))
                .ForMember(dst => dst.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dst => dst.Rolename, opt => opt.MapFrom(src => src.Rolename))
                .ForMember(dst => dst.ApplicationName, opt => opt.MapFrom(src => src.ApplicationName));

            Mapper.CreateMap<UserRoleViewModel, Common.Models.Account.UserRole>()
                .ForMember(dst => dst.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dst => dst.Rolename, opt => opt.MapFrom(src => src.Rolename))
                .ForMember(dst => dst.ApplicationName, opt => opt.MapFrom(src => src.ApplicationName));
        }
    }
}