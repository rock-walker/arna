// ==============================================================================================================
// Microsoft patterns & practices
// CQRS Journey project
// ==============================================================================================================
// ©2012 Microsoft. All rights reserved. Certain content used with permission from contributors
// http://go.microsoft.com/fwlink/p/?LinkID=258575
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance 
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software distributed under the License is 
// distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.
// ==============================================================================================================

namespace AP.Business.Registration.ReadModel.Implementation
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Infrastructure.BlobStorage;
    using Infrastructure.Serialization;
    using Registration.Handlers;
    using AP.Repository.Context;
    using AP.EntityModel.ReadModel;
    using Microsoft.EntityFrameworkCore;
    using EntityFramework.DbContextScope.Interfaces;
    using Microsoft.Extensions.Logging;

    public class OrderDao : IOrderDao
    {
        private IBlobStorage blobStorage;
        private ITextSerializer serializer;
        private IDbContextScopeFactory _factory;
        private readonly ILogger _logger;

        private readonly IAmbientDbContextLocator _ambientLocator;

        private WorkshopRegistrationDbContext DbContext
        {
            get
            {
                var dbContext = _ambientLocator.Get<WorkshopRegistrationDbContext>();
                if (dbContext == null)
                {
                    throw new InvalidOperationException("No ambient DbContext of type WorkshopDbContext found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");
                }
                return dbContext;
            }
        }

        public OrderDao(IAmbientDbContextLocator locator, 
            IBlobStorage blobStorage, 
            ITextSerializer serializer, 
            IDbContextScopeFactory factory,
            ILogger<OrderDao> logger)
        {
            this.blobStorage = blobStorage;
            this.serializer = serializer;

            if (locator == null)
            {
                throw new ArgumentNullException("ambientDbContextLocator");
            }

            _ambientLocator = locator;
            _factory = factory;
            _logger = logger;
        }

        public Guid? LocateOrder(Guid attendee, string accessCode)
        {
            var orderProjection = DbContext
                .Query<DraftOrder>()
                .Where(o => o.AttendeeID == attendee && o.AccessCode == accessCode)
                .Select(o => new { o.OrderID })
                .FirstOrDefault();

            if (orderProjection != null)
            {
                return orderProjection.OrderID;
            }

            return null;
        }

        public DraftOrder FindDraftOrder(Guid orderId)
        {
            using (var context = _factory.CreateReadOnly())
            {
                _logger.LogDebug(string.Format("DB Access: finding draft order ID {0}. Time: {1}", orderId, DateTime.UtcNow));
                return DbContext.Query<DraftOrder>().Include(x => x.Lines).FirstOrDefault(dto => dto.OrderID == orderId);
            }
        }

        public PricedOrder FindPricedOrder(Guid orderId)
        {
            using (var context = _factory.CreateReadOnly())
            {
                _logger.LogDebug(string.Format("DB Access: finding priced order ID {0}. Time: {1}", orderId, DateTime.UtcNow));
                return DbContext.Query<PricedOrder>().Include(x => x.Lines).FirstOrDefault(dto => dto.OrderId == orderId);
            }
        }

        public OrderAnchors FindOrderSeats(Guid assignmentsId)
        {
            return FindBlob<OrderAnchors>(AnchorAssignmentsViewModelGenerator.GetAnchorAssignmentsBlobId(assignmentsId));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "By design")]
        private T FindBlob<T>(string id)
            where T : class
        {
            var dto = this.blobStorage.Find(id).Result;
            if (dto == null)
            {
                return null;
            }

            using (var stream = new MemoryStream(dto))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                return (T)this.serializer.Deserialize(reader);
            }
        }
    }
}