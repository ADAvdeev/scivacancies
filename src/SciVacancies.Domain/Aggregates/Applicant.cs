﻿using SciVacancies.Domain.Core;
using SciVacancies.Domain.Events;

using System;
using System.Collections.Generic;

using CommonDomain.Core;

namespace SciVacancies.Domain.Aggregates
{
    public class Applicant : AggregateBase
    {
        private bool Deleted { get; set; }
        private List<Request> Requests { get; set; }
        private List<Notification> Notifications { get; set; }

        public Applicant()
        {

        }
        public Applicant(Guid guid)
        {
            RaiseEvent(new ApplicantCreated()
            {
                ApplicantGuid = guid
            });
        }

        public void Remove()
        {
            if (!Deleted)
            {
                RaiseEvent(new ApplicantRemoved()
                {
                    ApplicantGuid = this.Id
                });
            }
        }

        public void CreateRequest(Guid competitionGuid)
        {
            RaiseEvent(new RequestCreated()
            {
                RequestGuid = Guid.NewGuid(),
                CompetitionGuid = competitionGuid
            });
        }
        public void SendRequest(Guid requestGuid)
        {
            RaiseEvent(new RequestSent()
            {
                RequestGuid = requestGuid,
                CompetitionGuid = this.Requests.Find(f => f.RequestGuid == requestGuid).CompetitionGuid
            });
        }

        #region Apply-Handlers
        public void Apply(ApplicantCreated @event)
        {
            this.Id = @event.ApplicantGuid;
        }
        public void Apply(ApplicantRemoved @event)
        {
            this.Deleted = true;
        }
        
        public void Apply(RequestCreated @event)
        {
            this.Requests.Add(new Request()
            {
                RequestGuid = @event.RequestGuid,
                CompetitionGuid = @event.CompetitionGuid,
                Status = RequestStatus.InProcess
            });
        }
        public void Apply(RequestSent @event)
        {
            this.Requests.Find(f => f.RequestGuid == @event.RequestGuid).Status = RequestStatus.Sent;
        }
        #endregion
    }
}