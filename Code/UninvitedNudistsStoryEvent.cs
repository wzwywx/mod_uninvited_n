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
            float damage = rng.Range(0.1f, 0.5f);

            CmdSpawnBeing cmdSpawnBeing = new CmdSpawnBeing(EntityUtils.CenterOf(posIdx), The.Defs.Get(id), skipGreeting: false, null, damage, "Physical", isDamagePercentage: true);

            // TODO: To figure out for custom Persona use
            // cmdSpawnBeing.WithPersona(GenerateNudistPersona(Species.ForDef(id)));
            cmdSpawnBeing.Execute(S);

            Being being = cmdSpawnBeing.Being;
            being.Traits.Set(new List<Trait> { BeingTraits.Get("nudist"), BeingTraits.Get("enlightened") }, true);

            S.Situation.AddTension(0f, "Capsule Uninvited Nudists");
            logEntry = EventLogEntry.CreateFor(this, "evt.capsule.uninvited_nudists.title".T(), "evt.capsule.uninvited_nudists.description".T(), being.Definition.Preview, being.Id);
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