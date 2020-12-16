﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using LetsDoStuff.Domain;
using LetsDoStuff.Domain.Models;
using LetsDoStuff.WebApi.Services.DTO;
using LetsDoStuff.WebApi.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace LetsDoStuff.WebApi.Services
{
    public class ActivityManager : IActivityService
    {
        private readonly LdsContext db;

        public ActivityManager(LdsContext context)
        {
            db = context;
        }

        public List<ActivityResponse> GetAllActivities()
        {
            var activities = db.Activities
                .Select(a => new ActivityResponse()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Capacity = a.Capacity,
                    Creator = new ActivityCreatorResponse()
                    {
                        Name = a.Creator.FirstName + " " + a.Creator.LastName,
                        ProfileLink = a.Creator.ProfileLink
                    },
                    Date = a.Date,
                    Tags = a.Tags.Select(t => t.Name).ToList()
                }).OrderBy(o => o.Date);

            var orderedActiv = new List<ActivityResponse>();
            foreach (var item in activities)
            {
                orderedActiv.Add(item);
            }

            foreach (var item in orderedActiv)
            {
                Console.WriteLine(item.Date);
            }

            return orderedActiv;
        }

        public ActivityResponse GetActivityById(int id)
        {
            var activity = db.Activities.AsNoTracking()
                .Include(a => a.Tags)
                .Include(a => a.Creator)
                .FirstOrDefault(a => a.Id == id)
                ?? throw new ArgumentException($"There is no activity with id {id}");

            return new ActivityResponse()
            {
                Id = activity.Id,
                Name = activity.Name,
                Description = activity.Description,
                Capacity = activity.Capacity,
                Creator = new ActivityCreatorResponse()
                {
                    Name = activity.Creator.FirstName + " " + activity.Creator.LastName,
                    ProfileLink = activity.Creator.ProfileLink
                },
                Tags = activity.Tags
                    .Select(t => t.Name)
                    .ToList()
            };
        }

        public List<TagResponse> GetAvailableTags()
        {
            var tags = db.Tags.AsNoTracking()
                .Select(t => new TagResponse()
                {
                    Id = t.Id,
                    Name = t.Name
                }).ToList();

            return tags;
        }

        public void CreateActivity(CreateActivityCommand newActivity, int idUser)
        {
            var creator = db.Users.FirstOrDefault(u => u.Id == idUser);

            var activity = new Activity
            {
                Creator = creator,
                Name = newActivity.Name,
                Description = newActivity.Description,
                Capacity = newActivity.Capacity
            };

            foreach (int id in newActivity.TagIds)
            {
                var tag = db.Tags.Find(id);
                if (tag == null)
                {
                    throw new ArgumentException($"Tag with id {id} has not been found");
                }

                activity.Tags.Add(tag);
            }

            db.Activities.Add(activity);
            db.SaveChanges();
        }
    }
}
