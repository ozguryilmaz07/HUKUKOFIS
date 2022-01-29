﻿// -----------------------------------------------------------------------
// <copyright file="GroupInvoiceViewModel.cs" company="Nodine Legal, LLC">
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

namespace OpenLawOffice.Web.ViewModels.Billing
{
    using System;
    using AutoMapper;
    using OpenLawOffice.Common.Models;
    using System.Collections.Generic;

    public class GroupInvoiceViewModel
    {
        public Guid? Id { get; set; }
        public Contacts.ContactViewModel BillTo { get; set; }
        public DateTime Date { get; set; }
        public DateTime Due { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public string ExternalInvoiceId { get; set; }
        public string BillTo_NameLine1 { get; set; }
        public string BillTo_NameLine2 { get; set; }
        public string BillTo_AddressLine1 { get; set; }
        public string BillTo_AddressLine2 { get; set; }
        public string BillTo_City { get; set; }
        public string BillTo_State { get; set; }
        public string BillTo_Zip { get; set; }
        public BillingGroupViewModel BillingGroup { get; set; }

        public List<GroupInvoiceItemViewModel> Matters { get; set; }

        public GroupInvoiceViewModel()
        {
            Matters = new List<GroupInvoiceItemViewModel>();
        }
    }
}