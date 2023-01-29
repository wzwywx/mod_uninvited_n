using Game;
using Game.Procgen;
using Game.Utils;
using Game.Commands;
using Game.Data;
using Game.Story.Events;
using KL.Randomness;
using KL.Utils;
using UnityEngine;
using System.Collections.Generic;
using Game.AI.Traits;
using Game.Constants;

namespace UninvitedNudists
{
    public sealed class UninvitedNudistsStoryEvent : StoryEvent, ISeededEvent, IStoryEvent, ISaveable, IPositionalEvent
    {
        public const string Id = "UninvitedNudists";

        private static readonly StoryEventMeta meta = new StoryEventMeta
        {
            Id = Id,
            AutoEnd = true,
            Category = "Capsule",
            Create = () => new UninvitedNudistsStoryEvent()
        };

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Register()
        {
            StoryEvent.Register(meta);
        }

        private EventLogEntry logEntry;

        private int seed;

        private int posIdx;
        public override float TensionToAddOnStart => 0f;

        public override StoryEventMeta Meta => meta;

        public override EventLogEntry LogEntry => logEntry;
        private EventNotification notification;

        // TODO: Figure out how to use the generated nudist Persona to alter the traits 
        private Persona GenerateNudistPersona(Species species)
        {
            Persona persona = GenPersona.GenerateFor(S, S.Rng, species);

            // TODO: The following should work but somehow, extra traits are generated on top of the two 
            // persona.Species.Traits = new string[] { "nudist", "enlightened" };

            return persona;
        }

        public override void OnStart(long ticks)
        {
            Rng rng = new Rng(seed);

            string id = "Beings/Human01";
            float damage = rng.Range(0.1f, 0.25f);

            int maxNudists = rng.WeightedFrom<int>(new Dictionary<int, float> { { 5, 0.7f }, { 8, 0.3f }, { 10, 0.1f } });
            int minNudists = 3;
            int nudistsCount = rng.Range(minNudists, maxNudists);

            CmdSpawnBeing cmdSpawnBeing = new CmdSpawnBeing(EntityUtils.CenterOf(posIdx), The.Defs.Get(id), skipGreeting: false, null, damage, "Physical", isDamagePercentage: true);

            // TODO: To figure out for custom Persona use
            // cmdSpawnBeing.WithPersona(GenerateNudistPersona(Species.ForDef(id)));

            for (int i = 0; i < nudistsCount; i++)
            {
                cmdSpawnBeing.Execute(S);
                Being being = cmdSpawnBeing.Being;
                being.Traits.Set(new List<Trait> { BeingTraits.Get("nudist"), BeingTraits.Get("enlightened") }, true);
            }

            Being previewNudist = cmdSpawnBeing.Being;

            S.Situation.AddTension(0f, "Capsule Uninvited Nudists");

            string evTitle = "evt.capsule.uninvited_nudists.title";
            string evDesc = "evt.capsule.uninvited_nudists.description";

            logEntry = EventLogEntry.CreateFor(this, evTitle.T(nudistsCount), evDesc.T(), previewNudist.Definition.Preview, previewNudist.Id);
            S.Story.Log.AddLogEntry(logEntry);

            notification = EventNotification.Create(
               S.Ticks, UDB.Create(previewNudist.MainDataProvider,
                   UDBT.IEvent, previewNudist.Definition.Preview, evTitle.T(nudistsCount))
               .WithIconClickFunction(LogEntry.ShowDetails),
                   Priority.Normal, false);
            S.Sig.AddEvent.Send(this.notification);
        }
        public void SetPosIdx(int posIdx)
        {
            this.posIdx = posIdx;

        }

        public void SetSeed(int seed)
        {
            this.seed = seed;

        }
    }
}