using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace Stackpole.Models
{
    public static class Helpers
    {
        public static EntityState ConvertState(ObjectState objectState)
        {
            switch(objectState)
            {
                case ObjectState.Added:
                    return EntityState.Added;
                case ObjectState.Unchanged:
                    return EntityState.Unchanged;
                case ObjectState.Modified:
                    return EntityState.Modified;
               default:
                    return EntityState.Deleted;
            }
        }

        public static void ApplyStateChanges(this DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries<IObjectWithState>())
            {
                IObjectWithState stateInfo = entry.Entity;
                entry.State = ConvertState(stateInfo.ObjectState);
            }
        }
    }
}