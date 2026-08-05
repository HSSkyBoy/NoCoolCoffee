using System;
using System.Collections.Generic;
using GTA;
using GTA.Native;

namespace CinnamonCoffee
{
    public partial class CinnamonCoffee
    {
        private static string[] GetWalkAwayLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":
                    return new string[] {
                        "~r~" + n + ":~s~ \"Please just... leave me alone.\"",
                        "~r~" + n + ":~s~ \"I can't do this. I'm going.\"",
                    };
                case "Sweet":
                    return new string[] {
                        "~r~" + n + ":~s~ \"I really tried. I have to go now.\"",
                        "~r~" + n + ":~s~ \"Please don't follow me. I'm done.\"",
                    };
                case "Romantic":
                    return new string[] {
                        "~r~" + n + ":~s~ \"This isn't what I wanted. Goodbye.\"",
                        "~r~" + n + ":~s~ \"I deserve better than this. I'm leaving.\"",
                    };
                case "Needy":
                    return new string[] {
                        "~r~" + n + ":~s~ \"I can't believe you. I'm done.\"",
                        "~r~" + n + ":~s~ \"You really hurt me. I'm going.\"",
                    };
                case "Flirty":
                    return new string[] {
                        "~r~" + n + ":~s~ \"And just like that, you ruined it. Bye.\"",
                        "~r~" + n + ":~s~ \"Not cute anymore. I'm out.\"",
                    };
                case "Playful":
                    return new string[] {
                        "~r~" + n + ":~s~ \"Okay, game over. I'm leaving.\"",
                        "~r~" + n + ":~s~ \"Not funny anymore. We're done.\"",
                    };
                case "Party Girl":
                    return new string[] {
                        "~r~" + n + ":~s~ \"Ugh. You just killed my vibe. I'm out.\"",
                        "~r~" + n + ":~s~ \"Done. I've got better places to be.\"",
                    };
                case "Sarcastic":
                    return new string[] {
                        "~r~" + n + ":~s~ \"Oh wow. Shocking ending. I'm leaving.\"",
                        "~r~" + n + ":~s~ \"Great job. Really. Goodbye.\"",
                    };
                case "Cold":
                    return new string[] {
                        "~r~" + n + ":~s~ \"I'm done. Don't talk to me again.\"",
                        "~r~" + n + ":~s~ \"That's it. Walk away.\"",
                    };
                case "Independent":
                    return new string[] {
                        "~r~" + n + ":~s~ \"I don't need this. I'm gone.\"",
                        "~r~" + n + ":~s~ \"My time is too valuable for this.\"",
                    };
                case "Mysterious":
                    return new string[] {
                        "~r~" + n + ":~s~ \"Some things don't need explaining. This is over.\"",
                        "~r~" + n + ":~s~ \"I think we both know I'm leaving now.\"",
                    };
                case "Classy":
                    return new string[] {
                        "~r~" + n + ":~s~ \"I expected more. Clearly a mistake. Goodbye.\"",
                        "~r~" + n + ":~s~ \"This is beneath me. I'm leaving.\"",
                    };
                case "Gold Digger":
                    return new string[] {
                        "~r~" + n + ":~s~ \"You're not worth my time. Bye.\"",
                        "~r~" + n + ":~s~ \"I've got better options. Don't contact me.\"",
                    };
                case "Manipulative":
                    return new string[] {
                        "~r~" + n + ":~s~ \"You played yourself. I'm done here.\"",
                        "~r~" + n + ":~s~ \"This conversation is no longer useful to me.\"",
                    };
                case "Street Smart":
                    return new string[] {
                        "~r~" + n + ":~s~ \"I see what this is. I'm out.\"",
                        "~r~" + n + ":~s~ \"You're not worth the drama. Gone.\"",
                    };
                case "Jealous":
                    return new string[] {
                        "~r~" + n + ":~s~ \"I can't even look at you right now. Leave me alone.\"",
                        "~r~" + n + ":~s~ \"Forget it. I'm done with this.\"",
                    };
                case "Dominant":
                    return new string[] {
                        "~r~" + n + ":~s~ \"I don't tolerate this. We're done.\"",
                        "~r~" + n + ":~s~ \"You had your chance. Walk away.\"",
                    };
                case "Aggressive":
                    return new string[] {
                        "~r~" + n + ":~s~ \"Get out of my face. Now.\"",
                        "~r~" + n + ":~s~ \"Don't push me. I'm leaving before this gets worse.\"",
                    };
                case "Chaotic":
                    return new string[] {
                        "~r~" + n + ":~s~ \"I'm bored of this. Bye.\"",
                        "~r~" + n + ":~s~ \"You're too much. Or not enough. Either way, I'm gone.\"",
                    };
                case "Unstable":
                    return new string[] {
                        "~r~" + n + ":~s~ \"I can't — I just can't. I'm leaving.\"",
                        "~r~" + n + ":~s~ \"Don't follow me. I mean it.\"",
                    };
                default:
                    return new string[] {
                        "~r~" + n + ":~s~ \"I'm done. Goodbye.\"",
                        "~r~" + n + ":~s~ \"That's it. I'm leaving.\"",
                    };
            }
        }

        /// <summary>Personality-matched "not out here, too risky" lines.</summary>
        private static string[] GetRiskyOutdoorLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":
                    return new string[] { "~r~"+n+":~s~ \"P-please, not here! Someone could see us!\"", "~r~"+n+":~s~ \"I can't do that out in the open...\"" };
                case "Sweet":
                    return new string[] { "~r~"+n+":~s~ \"Not out here, okay? Can we find somewhere private?\"", "~r~"+n+":~s~ \"I'd love to but not where people can see us!\"" };
                case "Romantic":
                    return new string[] { "~r~"+n+":~s~ \"Not like this. I want somewhere private, just us.\"", "~r~"+n+":~s~ \"This should be somewhere special, not out here.\"" };
                case "Needy":
                    return new string[] { "~r~"+n+":~s~ \"Please not out here... I don't want people to see.\"", "~r~"+n+":~s~ \"Can we go somewhere private? I just want it to be us.\"" };
                case "Flirty":
                    return new string[] { "~r~"+n+":~s~ \"Ooh, bold — but not out here. Find us a spot.\"", "~r~"+n+":~s~ \"I'm into it, just not where anyone can watch.\"" };
                case "Playful":
                    return new string[] { "~r~"+n+":~s~ \"Haha okay but NOT out here. Get creative.\"", "~r~"+n+":~s~ \"Points for nerve, but find somewhere private first.\"" };
                case "Party Girl":
                    return new string[] { "~r~"+n+":~s~ \"Not out in the open! At least find a car.\"", "~r~"+n+":~s~ \"I'm down but not where everyone can see us.\"" };
                case "Sarcastic":
                    return new string[] { "~r~"+n+":~s~ \"Oh sure, right here on the street. Great plan.\"", "~r~"+n+":~s~ \"Yeah, no. Find somewhere that isn't literally public.\"" };
                case "Cold":
                    return new string[] { "~r~"+n+":~s~ \"Not in public. Find somewhere else.\"", "~r~"+n+":~s~ \"I don't do this where people can see.\"" };
                case "Independent":
                    return new string[] { "~r~"+n+":~s~ \"Not out here. I'm not doing this where anyone can see.\"", "~r~"+n+":~s~ \"Private means private. Figure it out.\"" };
                case "Classy":
                    return new string[] { "~r~"+n+":~s~ \"Absolutely not out here. I have standards.\"", "~r~"+n+":~s~ \"Find somewhere discreet or don't bother.\"" };
                case "Gold Digger":
                    return new string[] { "~r~"+n+":~s~ \"Not out here. I'm not cheap AND careless.\"", "~r~"+n+":~s~ \"I need privacy for this. Go find somewhere.\"" };
                case "Manipulative":
                    return new string[] { "~r~"+n+":~s~ \"Not here. Somewhere private, or the deal's off.\"", "~r~"+n+":~s~ \"I'm not doing this in public. You know better.\"" };
                case "Street Smart":
                    return new string[] { "~r~"+n+":~s~ \"Not out in the open. You trying to get us caught?\"", "~r~"+n+":~s~ \"Find cover. I'm not doing this where anyone can see.\"" };
                case "Mysterious":
                    return new string[] { "~r~"+n+":~s~ \"Not here. Somewhere no one's watching.\"", "~r~"+n+":~s~ \"I prefer privacy for this kind of thing.\"" };
                case "Jealous":
                    return new string[] { "~r~"+n+":~s~ \"Not where everyone can see! Are you serious?\"", "~r~"+n+":~s~ \"Find somewhere private. I'm not doing this out here.\"" };
                case "Dominant":
                    return new string[] { "~r~"+n+":~s~ \"Not in public. Get us somewhere private. Now.\"", "~r~"+n+":~s~ \"I don't perform for strangers. Find a spot.\"" };
                case "Aggressive":
                    return new string[] { "~r~"+n+":~s~ \"Not out here. Move it.\"", "~r~"+n+":~s~ \"Find a private spot or forget it.\"" };
                case "Chaotic":
                    return new string[] { "~r~"+n+":~s~ \"Tempting, but even I have limits. Not in public.\"", "~r~"+n+":~s~ \"Bold. Still no. Find somewhere less open.\"" };
                case "Unstable":
                    return new string[] { "~r~"+n+":~s~ \"Not here, not here — people can SEE us!\"", "~r~"+n+":~s~ \"Somewhere private, please. I can't do this out here.\"" };
                default:
                    return new string[] { "~r~"+n+":~s~ \"Not out here. Get us somewhere private.\"", "~r~"+n+":~s~ \"I don't do this in public.\"" };
            }
        }

        /// <summary>Personality-matched "you can't afford the kissing surcharge" lines.</summary>
        private static string[] GetCantAffordKissLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":       return new string[] { "~r~"+n+":~s~ \"S-sorry, you don't have enough for that...\"", "~r~"+n+":~s~ \"You're a little short. Come back when you've got it.\"" };
                case "Sweet":     return new string[] { "~r~"+n+":~s~ \"Aww, you don't have enough for that right now.\"", "~r~"+n+":~s~ \"I wish, but you can't afford it sweetie.\"" };
                case "Romantic":  return new string[] { "~r~"+n+":~s~ \"I'd love to, but you can't cover it right now.\"", "~r~"+n+":~s~ \"Come back when you've got enough.\"" };
                case "Needy":     return new string[] { "~r~"+n+":~s~ \"You're short... come back when you have enough?\"", "~r~"+n+":~s~ \"I really want to but you can't afford it.\"" };
                case "Flirty":    return new string[] { "~r~"+n+":~s~ \"Nice try, but you're a little short.\"", "~r~"+n+":~s~ \"Come back with more cash and we'll talk.\"" };
                case "Playful":   return new string[] { "~r~"+n+":~s~ \"Ha! Not enough. Come back when you're loaded.\"", "~r~"+n+":~s~ \"You're short, babe. Try again later.\"" };
                case "Party Girl":return new string[] { "~r~"+n+":~s~ \"Not enough, sorry. Get more cash first.\"", "~r~"+n+":~s~ \"Come back when you can actually afford it.\"" };
                case "Sarcastic": return new string[] { "~r~"+n+":~s~ \"Oh wow, not enough. Shocking.\"", "~r~"+n+":~s~ \"Come back when your wallet matches your ambition.\"" };
                case "Cold":      return new string[] { "~r~"+n+":~s~ \"You can't afford it. Move on.\"", "~r~"+n+":~s~ \"Not enough. Don't waste my time.\"" };
                case "Independent":return new string[] { "~r~"+n+":~s~ \"You're short. Come back when you have it.\"", "~r~"+n+":~s~ \"Can't afford it. That's that.\"" };
                case "Classy":    return new string[] { "~r~"+n+":~s~ \"You're not in a position to afford this. Come back.\"", "~r~"+n+":~s~ \"That requires more than you're carrying.\"" };
                case "Gold Digger":return new string[] { "~r~"+n+":~s~ \"Short on cash? Not my problem. Come back later.\"", "~r~"+n+":~s~ \"Don't waste my time if you can't pay.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"Interesting. You want it but can't afford it.\"", "~r~"+n+":~s~ \"Come back when you're less broke.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"You're light. Come back with the full amount.\"", "~r~"+n+":~s~ \"Don't come to me broke. Get the cash.\"" };
                case "Dominant":  return new string[] { "~r~"+n+":~s~ \"You can't afford me right now. Come back prepared.\"", "~r~"+n+":~s~ \"Not enough. Don't ask until you have it.\"" };
                case "Aggressive":return new string[] { "~r~"+n+":~s~ \"Broke. Get out.\"", "~r~"+n+":~s~ \"Pathetic. Come back with real money.\"" };
                case "Chaotic":   return new string[] { "~r~"+n+":~s~ \"Ha! Short. Get more and maybe I'll say yes.\"", "~r~"+n+":~s~ \"Not enough. Funny though. Try again.\"" };
                case "Unstable":  return new string[] { "~r~"+n+":~s~ \"You DON'T have enough — come back!\"", "~r~"+n+":~s~ \"Not enough. Come back. Please.\"" };
                default:          return new string[] { "~r~"+n+":~s~ \"You can't afford it. Come back with more.\"", "~r~"+n+":~s~ \"Not enough cash. Move on.\"" };
            }
        }

        /// <summary>Personality-matched "I'll kiss you but it'll cost extra" lines.</summary>
        private static string[] GetGreedKissLines(string name, string personality, int cost)
        {
            string n = name; string c = "$" + cost;
            switch (personality)
            {
                case "Shy":       return new string[] { "~g~"+n+":~s~ \"I-I'll do it, but it's "+c+" extra, okay?\"", "~g~"+n+":~s~ \"Just this once... but that's "+c+".\"" };
                case "Sweet":     return new string[] { "~g~"+n+":~s~ \"Okay, okay... but it'll cost you "+c+", cutie.\"", "~g~"+n+":~s~ \"Aw, fine. Just this once — you're paying "+c+"!\"" };
                case "Romantic":  return new string[] { "~g~"+n+":~s~ \"I'll make an exception. But that's "+c+".\"", "~g~"+n+":~s~ \"Fine... but only because it's you. "+c+".\"" };
                case "Needy":     return new string[] { "~g~"+n+":~s~ \"For you? Yes. But please — "+c+".\"", "~g~"+n+":~s~ \"I'll do it. I just need "+c+". Don't tell anyone.\"" };
                case "Flirty":    return new string[] { "~g~"+n+":~s~ \"Ooh, sure — but that's "+c+" extra.\"", "~g~"+n+":~s~ \"For the right price I'll make an exception. "+c+".\"" };
                case "Playful":   return new string[] { "~g~"+n+":~s~ \"Haha okay fine — "+c+" extra though!\"", "~g~"+n+":~s~ \"Fine, twist my arm. "+c+".\"" };
                case "Party Girl":return new string[] { "~g~"+n+":~s~ \"Sure, but you're paying "+c+" for it.\"", "~g~"+n+":~s~ \"I can do that — "+c+", upfront.\"" };
                case "Sarcastic": return new string[] { "~g~"+n+":~s~ \"Sure. "+c+". Don't read into it.\"", "~g~"+n+":~s~ \"Fine. "+c+". Business only.\"" };
                case "Cold":      return new string[] { "~g~"+n+":~s~ \""+c+". That's the price. Take it or leave it.\"", "~g~"+n+":~s~ \"Don't read into it. "+c+", upfront.\"" };
                case "Gold Digger":return new string[] { "~g~"+n+":~s~ \"Everything has a price. This one's "+c+".\"", "~g~"+n+":~s~ \"For "+c+"? Fine. Don't expect warmth.\"" };
                case "Manipulative":return new string[] { "~g~"+n+":~s~ \"I'll make an exception — for "+c+".\"", "~g~"+n+":~s~ \"You want it? "+c+". Simple.\"" };
                case "Street Smart":return new string[] { "~g~"+n+":~s~ \"Alright, but that's "+c+" extra. Deal?\"", "~g~"+n+":~s~ \""+c+". That's the rate. Pay up.\"" };
                case "Dominant":  return new string[] { "~g~"+n+":~s~ \"You want it, you pay "+c+" for it. Clear?\"", "~g~"+n+":~s~ \"Fine. "+c+". My rules.\"" };
                case "Aggressive":return new string[] { "~g~"+n+":~s~ \"Money's money. "+c+". Don't enjoy it.\"", "~g~"+n+":~s~ \"This means nothing. "+c+", upfront.\"" };
                case "Chaotic":   return new string[] { "~g~"+n+":~s~ \"Sure why not. "+c+". Random Tuesday.\"", "~g~"+n+":~s~ \"Breaking my own rules for "+c+". Fine.\"" };
                case "Unstable":  return new string[] { "~g~"+n+":~s~ \"Okay YES — but "+c+"! Right now!\"", "~g~"+n+":~s~ \"I'll do it. "+c+". Just — yes.\"" };
                default:          return new string[] { "~g~"+n+":~s~ \"That'll cost you "+c+" extra.\"", "~g~"+n+":~s~ \"Fine. "+c+". That's my price.\"" };
            }
        }

        /// <summary>Personality-matched "I don't kiss casually" refusal lines.</summary>
        private static string[] GetNoKissLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":       return new string[] { "~r~"+n+":~s~ \"I-I can't kiss you. That's too much for me.\"", "~r~"+n+":~s~ \"Sorry... kissing is really personal.\"" };
                case "Sweet":     return new string[] { "~r~"+n+":~s~ \"Kissing's really intimate for me. I'm not ready.\"", "~r~"+n+":~s~ \"I really like you, but kissing is a bit much. Sorry.\"" };
                case "Romantic":  return new string[] { "~r~"+n+":~s~ \"Kissing means something real to me. Not like this.\"", "~r~"+n+":~s~ \"I only kiss someone I truly care about.\"" };
                case "Needy":     return new string[] { "~r~"+n+":~s~ \"I want to... but kissing is too personal right now.\"", "~r~"+n+":~s~ \"Not kissing. That's too close for me.\"" };
                case "Flirty":    return new string[] { "~r~"+n+":~s~ \"Kissing's off limits. Everything else, maybe.\"", "~r~"+n+":~s~ \"I don't kiss casually. That's my one rule.\"" };
                case "Playful":   return new string[] { "~r~"+n+":~s~ \"Not kissing! That's my thing, okay?\"", "~r~"+n+":~s~ \"Nice try, but lips are off limits.\"" };
                case "Party Girl":return new string[] { "~r~"+n+":~s~ \"No kissing, babe. That's where I draw the line.\"", "~r~"+n+":~s~ \"Everything else is fine, but not that.\"" };
                case "Sarcastic": return new string[] { "~r~"+n+":~s~ \"Kissing? No. I have standards somewhere.\"", "~r~"+n+":~s~ \"That's personal. Surprisingly, yes.\"" };
                case "Cold":      return new string[] { "~r~"+n+":~s~ \"No kissing. Don't ask.\"", "~r~"+n+":~s~ \"That's too personal. The answer is no.\"" };
                case "Independent":return new string[] { "~r~"+n+":~s~ \"I don't kiss unless I want to. I don't want to.\"", "~r~"+n+":~s~ \"Kissing is my call. Not right now.\"" };
                case "Classy":    return new string[] { "~r~"+n+":~s~ \"Kissing is reserved for someone I actually care about.\"", "~r~"+n+":~s~ \"That's not on the table. I have boundaries.\"" };
                case "Gold Digger":return new string[] { "~r~"+n+":~s~ \"Kissing's not included. It's not negotiable.\"", "~r~"+n+":~s~ \"That costs extra — and not in cash. No.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"Kissing? No. That would give you the wrong idea.\"", "~r~"+n+":~s~ \"Keep your expectations in check. No kissing.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"No kissing. That's just how I do things.\"", "~r~"+n+":~s~ \"Lips are off limits. Don't push it.\"" };
                case "Dominant":  return new string[] { "~r~"+n+":~s~ \"I'll decide if and when kissing happens. Not today.\"", "~r~"+n+":~s~ \"No kissing. That's final.\"" };
                case "Aggressive":return new string[] { "~r~"+n+":~s~ \"I don't kiss. Period.\"", "~r~"+n+":~s~ \"Absolutely not. Don't bring it up again.\"" };
                case "Chaotic":   return new string[] { "~r~"+n+":~s~ \"Huh, turns out I have a limit. No kissing.\"", "~r~"+n+":~s~ \"Everything but that. Don't ask why.\"" };
                case "Unstable":  return new string[] { "~r~"+n+":~s~ \"No — NO kissing. That's MINE. Don't.\"", "~r~"+n+":~s~ \"Kissing is off limits. Please.\"" };
                default:          return new string[] { "~r~"+n+":~s~ \"No kissing. That's personal.\"", "~r~"+n+":~s~ \"I don't kiss. That's my rule.\"" };
            }
        }

        /// <summary>Personality-matched "no, pull out" (finish-ask denied) lines.</summary>
        private static string[] GetFinishAskNoLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":       return new string[] { "~r~"+n+":~s~ \"N-no... please just pull out.\"", "~r~"+n+":~s~ \"I'm not ready for that.\"" };
                case "Sweet":     return new string[] { "~r~"+n+":~s~ \"Aww... not yet, okay? Sorry.\"", "~r~"+n+":~s~ \"I wish I could say yes... not today.\"" };
                case "Romantic":  return new string[] { "~r~"+n+":~s~ \"Not like this. Pull out.\"", "~r~"+n+":~s~ \"I want it to be special first. Not now.\"" };
                case "Needy":     return new string[] { "~r~"+n+":~s~ \"Please don't... I'm not okay with that yet.\"", "~r~"+n+":~s~ \"Not yet. I need more time.\"" };
                case "Flirty":    return new string[] { "~r~"+n+":~s~ \"Hmm... not today, babe.\"", "~r~"+n+":~s~ \"Not this time. Ask me again sometime.\"" };
                case "Playful":   return new string[] { "~r~"+n+":~s~ \"Ooh, bold ask. Still no though.\"", "~r~"+n+":~s~ \"Ha! Nice try. Pull out.\"" };
                case "Party Girl":{ int _hr0 = Function.Call<int>(Hash.GET_CLOCK_HOURS); bool _nt0 = _hr0 >= 21 || _hr0 < 3; return new string[] { "~r~"+n+":~s~ " + (_nt0 ? "\"Not tonight. Pull out.\"" : "\"Not right now. Pull out.\""), "~r~"+n+":~s~ \"No way, not right now.\"" }; }
                case "Sarcastic": return new string[] { "~r~"+n+":~s~ \"Wow. Ambitious. No.\"", "~r~"+n+":~s~ \"I'll give you points for asking. Still no.\"" };
                case "Cold":      return new string[] { "~r~"+n+":~s~ \"No. Pull out.\"", "~r~"+n+":~s~ \"I don't think so.\"" };
                case "Independent":return new string[] { "~r~"+n+":~s~ \"My body, my rules. Pull out.\"", "~r~"+n+":~s~ \"No. And I mean no.\"" };
                case "Classy":    return new string[] { "~r~"+n+":~s~ \"That's not something I'd agree to.\"", "~r~"+n+":~s~ \"Absolutely not. Pull out now.\"" };
                case "Gold Digger":return new string[] { "~r~"+n+":~s~ \"That's not included. Pull out.\"", "~r~"+n+":~s~ \"No. That costs a lot more than this.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"Not today. Pull out.\"", "~r~"+n+":~s~ \"No. I'll let you know when the answer changes.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"Nah. Pull out.\"", "~r~"+n+":~s~ \"I'm not stupid. No.\"" };
                case "Dominant":  return new string[] { "~r~"+n+":~s~ \"I didn't say you could. Pull out.\"", "~r~"+n+":~s~ \"No. My call, not yours.\"" };
                case "Aggressive":return new string[] { "~r~"+n+":~s~ \"Absolutely not. Pull out now.\"", "~r~"+n+":~s~ \"No. Don't even think about it.\"" };
                case "Chaotic":   return new string[] { "~r~"+n+":~s~ \"Ha. No. Maybe another time. Pull out.\"", "~r~"+n+":~s~ \"Not today. Random decision.\"" };
                case "Unstable":  return new string[] { "~r~"+n+":~s~ \"No — NO — pull out!\"", "~r~"+n+":~s~ \"You better not. I swear.\"" };
                default:          return new string[] { "~r~"+n+":~s~ \"No. Pull out.\"", "~r~"+n+":~s~ \"Not happening.\"" };
            }
        }

        /// <summary>Personality-matched "you did it without asking" reaction lines.</summary>
        private static string[] GetFinishForcedLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":       return new string[] { "~r~"+n+":~s~ \"You... you didn't even ask me!\"", "~r~"+n+":~s~ \"That was not okay. At all.\"" };
                case "Sweet":     return new string[] { "~r~"+n+":~s~ \"Hey — you should have asked first!\"", "~r~"+n+":~s~ \"That was really not okay. You know that, right?\"" };
                case "Romantic":  return new string[] { "~r~"+n+":~s~ \"That was supposed to be my choice.\"", "~r~"+n+":~s~ \"You had no right to do that without asking.\"" };
                case "Needy":     return new string[] { "~r~"+n+":~s~ \"How could you do that?! I trusted you!\"", "~r~"+n+":~s~ \"You should have ASKED me first!\"" };
                case "Flirty":    return new string[] { "~r~"+n+":~s~ \"Uh — that was not your call!\"", "~r~"+n+":~s~ \"You should have asked! That's basic.\"" };
                case "Playful":   return new string[] { "~r~"+n+":~s~ \"Hey! Not cool. You need to ask.\"", "~r~"+n+":~s~ \"Ugh, seriously?! Ask next time!\"" };
                case "Party Girl":return new string[] { "~r~"+n+":~s~ \"What the hell?! You didn't ask!\"", "~r~"+n+":~s~ \"That was NOT okay!\"" };
                case "Sarcastic": return new string[] { "~r~"+n+":~s~ \"Oh wow. Consent is apparently optional for you.\"", "~r~"+n+":~s~ \"Great. Just great. You should have asked.\"" };
                case "Cold":      return new string[] { "~r~"+n+":~s~ \"That wasn't your call to make.\"", "~r~"+n+":~s~ \"You should have asked. That's not okay.\"" };
                case "Independent":return new string[] { "~r~"+n+":~s~ \"You don't get to make that choice for me.\"", "~r~"+n+":~s~ \"MY body. You ask. Got it?\"" };
                case "Classy":    return new string[] { "~r~"+n+":~s~ \"That was disgraceful. You should have asked.\"", "~r~"+n+":~s~ \"Completely unacceptable.\"" };
                case "Gold Digger":return new string[] { "~r~"+n+":~s~ \"That was not included and you KNOW it.\"", "~r~"+n+":~s~ \"You're going to pay for that one way or another.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"You made a mistake just now.\"", "~r~"+n+":~s~ \"You should have asked. Remember that.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"What is wrong with you? You ask first.\"", "~r~"+n+":~s~ \"That was way out of line.\"" };
                case "Dominant":  return new string[] { "~r~"+n+":~s~ \"I didn't give you permission. EVER ask first.\"", "~r~"+n+":~s~ \"You do NOT do that without my say-so.\"" };
                case "Aggressive":return new string[] { "~r~"+n+":~s~ \"Are you KIDDING me right now?!\"", "~r~"+n+":~s~ \"Don't you EVER do that again.\"" };
                case "Chaotic":   return new string[] { "~r~"+n+":~s~ \"Ha — wait, no. You need to ASK.\"", "~r~"+n+":~s~ \"Even I have rules. You broke one.\"" };
                case "Unstable":  return new string[] { "~r~"+n+":~s~ \"I will KILL you.\"", "~r~"+n+":~s~ \"Get away from me. RIGHT NOW.\"" };
                default:          return new string[] { "~r~"+n+":~s~ \"What the hell?!\"", "~r~"+n+":~s~ \"You should have asked first.\"" };
            }
        }

        /// <summary>Personality-matched lines when A-Life hooker terminates arrangement after a forced finish.</summary>
        private static string[] GetForcedFinishHookerLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":        return new string[] { "~r~"+n+":~s~ \"I said no. We're done. Please don't contact me again.\"", "~r~"+n+":~s~ \"That was a violation. I'm out.\"" };
                case "Sweet":      return new string[] { "~r~"+n+":~s~ \"How could you? Arrangement's over. Don't come back.\"", "~r~"+n+":~s~ \"I said no. We're done.\"" };
                case "Romantic":   return new string[] { "~r~"+n+":~s~ \"You destroyed this. We're done. Don't come back.\"", "~r~"+n+":~s~ \"I said no. Arrangement's over.\"" };
                case "Needy":      return new string[] { "~r~"+n+":~s~ \"How could you do this?! We're done. Don't ever call me.\"", "~r~"+n+":~s~ \"I trusted you. Arrangement's over.\"" };
                case "Flirty":     return new string[] { "~r~"+n+":~s~ \"Not cute. We're done. Arrangement's over.\"", "~r~"+n+":~s~ \"That crossed a line. Don't contact me again.\"" };
                case "Playful":    return new string[] { "~r~"+n+":~s~ \"NOT okay. We're done. Don't come back.\"", "~r~"+n+":~s~ \"You crossed a line. Arrangement's over.\"" };
                case "Party Girl": return new string[] { "~r~"+n+":~s~ \"What the hell?! We're done. Arrangement's over.\"", "~r~"+n+":~s~ \"I said no! Don't come back.\"" };
                case "Sarcastic":  return new string[] { "~r~"+n+":~s~ \"I said no. You did it anyway. We're done.\"", "~r~"+n+":~s~ \"Arrangement's over. And don't call me.\"" };
                case "Cold":       return new string[] { "~r~"+n+":~s~ \"I said no. We're done. Don't contact me.\"", "~r~"+n+":~s~ \"Arrangement ends now. Get out.\"" };
                case "Independent":return new string[] { "~r~"+n+":~s~ \"My body. My rules. We're done.\"", "~r~"+n+":~s~ \"Arrangement's over. Don't come back.\"" };
                case "Classy":     return new string[] { "~r~"+n+":~s~ \"Completely unacceptable. Arrangement terminated immediately.\"", "~r~"+n+":~s~ \"Don't ever contact me again.\"" };
                case "Gold Digger":return new string[] { "~r~"+n+":~s~ \"That was NOT included and you KNOW it. We're DONE.\"", "~r~"+n+":~s~ \"Arrangement's over. Don't come back.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"You just made a very bad decision. We're done.\"", "~r~"+n+":~s~ \"Arrangement's over. Remember what you did.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"I said no. You crossed the line. We're done.\"", "~r~"+n+":~s~ \"Don't come near me again. Arrangement's over.\"" };
                case "Mysterious": return new string[] { "~r~"+n+":~s~ \"I said no. Now we're done.\"", "~r~"+n+":~s~ \"Arrangement's over. Don't contact me.\"" };
                case "Jealous":    return new string[] { "~r~"+n+":~s~ \"You crossed a line and you KNOW it! We are DONE.\"", "~r~"+n+":~s~ \"Arrangement's over! Don't ever come back.\"" };
                case "Dominant":   return new string[] { "~r~"+n+":~s~ \"I said no. You had no right. We're done.\"", "~r~"+n+":~s~ \"Arrangement ends now. Get out.\"" };
                case "Aggressive": return new string[] { "~r~"+n+":~s~ \"What the hell?! We're done. Don't come back.\"", "~r~"+n+":~s~ \"You just crossed a line. Arrangement's over.\"" };
                case "Chaotic":    return new string[] { "~r~"+n+":~s~ \"Ha — wait, no. That was NOT okay. We're done.\"", "~r~"+n+":~s~ \"You broke the one rule. Arrangement's over.\"" };
                case "Unstable":   return new string[] { "~r~"+n+":~s~ \"I SAID NO. WE ARE DONE. DON'T COME BACK.\"", "~r~"+n+":~s~ \"Arrangement's over. I'm out.\"" };
                default:           return new string[] { "~r~"+n+":~s~ \"What the hell?! We're done. Don't ever contact me again.\"", "~r~"+n+":~s~ \"I said no. Arrangement's over.\"" };
            }
        }

        /// <summary>Personality-matched lines when A-Life hooker terminates arrangement because client can't pay.</summary>
        private static string[] GetBrokeHookerTerminateLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":        return new string[] { "~r~"+n+":~s~ \"I... I can't keep doing this. You can't even pay. We're done.\"", "~r~"+n+":~s~ \"Please don't contact me again.\"" };
                case "Sweet":      return new string[] { "~r~"+n+":~s~ \"I really wanted this to work. But no money means we're done.\"", "~r~"+n+":~s~ \"Don't come back. I'm sorry.\"" };
                case "Romantic":   return new string[] { "~r~"+n+":~s~ \"I thought you were different. No money? We're done.\"", "~r~"+n+":~s~ \"Don't call me again.\"" };
                case "Needy":      return new string[] { "~r~"+n+":~s~ \"You can't even pay me? Don't contact me again.\"", "~r~"+n+":~s~ \"I needed you to come through. We're done.\"" };
                case "Flirty":     return new string[] { "~r~"+n+":~s~ \"Really? No money? Arrangement's over.\"", "~r~"+n+":~s~ \"Don't come back. That's embarrassing.\"" };
                case "Playful":    return new string[] { "~r~"+n+":~s~ \"No cash, no deal. We're done. Bye!\"", "~r~"+n+":~s~ \"Out of money, out of luck. Don't come back.\"" };
                case "Party Girl": return new string[] { "~r~"+n+":~s~ \"No money? No deal. Don't call me.\"", "~r~"+n+":~s~ \"That's embarrassing. We're done.\"" };
                case "Sarcastic":  return new string[] { "~r~"+n+":~s~ \"Oh wow. No money. Shocking. We're done.\"", "~r~"+n+":~s~ \"Arrangement's over. And don't call me.\"" };
                case "Cold":       return new string[] { "~r~"+n+":~s~ \"No money, no deal. We're done.\"", "~r~"+n+":~s~ \"Don't come back without cash.\"" };
                case "Independent":return new string[] { "~r~"+n+":~s~ \"I don't work for free. Arrangement's over.\"", "~r~"+n+":~s~ \"Don't come back.\"" };
                case "Classy":     return new string[] { "~r~"+n+":~s~ \"This is unacceptable. Arrangement terminated.\"", "~r~"+n+":~s~ \"Don't contact me again without the funds.\"" };
                case "Gold Digger":return new string[] { "~r~"+n+":~s~ \"Are you SERIOUS?! No money?! We're DONE.\"", "~r~"+n+":~s~ \"Don't ever call me again without cash.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"Interesting. You can't pay. We're done.\"", "~r~"+n+":~s~ \"Don't bother coming back.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"You came up short. Arrangement's over.\"", "~r~"+n+":~s~ \"Don't come near me without the full amount.\"" };
                case "Mysterious": return new string[] { "~r~"+n+":~s~ \"I gave you a chance. You wasted it. We're done.\"", "~r~"+n+":~s~ \"Don't contact me again.\"" };
                case "Jealous":    return new string[] { "~r~"+n+":~s~ \"You couldn't even PAY me?! Are you serious?! Done!\"", "~r~"+n+":~s~ \"Arrangement's over. I deserved better.\"" };
                case "Dominant":   return new string[] { "~r~"+n+":~s~ \"You can't pay? We're done. End of discussion.\"", "~r~"+n+":~s~ \"Arrangement ends now. My call.\"" };
                case "Aggressive": return new string[] { "~r~"+n+":~s~ \"No money?! Get out. Don't come back.\"", "~r~"+n+":~s~ \"We're done. Don't push your luck.\"" };
                case "Chaotic":    return new string[] { "~r~"+n+":~s~ \"Ha! Broke! We're done. Arrangement's over!\"", "~r~"+n+":~s~ \"No cash, no deal. See ya.\"" };
                case "Unstable":   return new string[] { "~r~"+n+":~s~ \"You DON'T have the money?! We are DONE.\"", "~r~"+n+":~s~ \"Don't come back. EVER.\"" };
                default:           return new string[] { "~r~"+n+":~s~ \"You can't even pay? We're done. Don't come back.\"", "~r~"+n+":~s~ \"No money, no deal. Arrangement's over.\"" };
            }
        }

        /// <summary>Personality-matched lines when she forgives the client for not being able to pay.</summary>
        private static string[] GetBrokeForgiveLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":        return new string[] { "~g~"+n+":~s~ \"O-okay... just this once. Please don't do it again.\"", "~g~"+n+":~s~ \"I'll let it go. But please have the money next time.\"" };
                case "Sweet":      return new string[] { "~g~"+n+":~s~ \"...Fine. Just this once. Don't make a habit of it, okay?\"", "~g~"+n+":~s~ \"I'll let it slide. But you owe me.\"" };
                case "Romantic":   return new string[] { "~g~"+n+":~s~ \"...Because it's you. Just this once.\"", "~g~"+n+":~s~ \"Don't make me regret this.\"" };
                case "Needy":      return new string[] { "~g~"+n+":~s~ \"Fine. But please have it next time. Please.\"", "~g~"+n+":~s~ \"I'll let it go this time. Don't let me down again.\"" };
                case "Flirty":     return new string[] { "~g~"+n+":~s~ \"Ugh, fine. You're lucky you're charming.\"", "~g~"+n+":~s~ \"Just this once. Don't push it.\"" };
                case "Playful":    return new string[] { "~g~"+n+":~s~ \"Haha okay fine, I'll let it slide. This once!\"", "~g~"+n+":~s~ \"You're lucky I'm in a good mood.\"" };
                case "Party Girl": return new string[] { "~g~"+n+":~s~ \"Ugh, fine. But seriously, bring the cash next time.\"", "~g~"+n+":~s~ \"I'll let it go. Don't make it a thing.\"" };
                case "Sarcastic":  return new string[] { "~g~"+n+":~s~ \"Oh fine. I'll let it slide. Shocking generosity from me.\"", "~g~"+n+":~s~ \"You owe me. Just so you know.\"" };
                case "Cold":       return new string[] { "~g~"+n+":~s~ \"Fine. This time. Don't test me again.\"", "~g~"+n+":~s~ \"I'll let it slide. Once.\"" };
                case "Independent":return new string[] { "~g~"+n+":~s~ \"Fine. But I don't do this twice.\"", "~g~"+n+":~s~ \"Consider yourself lucky.\"" };
                case "Classy":     return new string[] { "~g~"+n+":~s~ \"I'll make an exception. This once.\"", "~g~"+n+":~s~ \"Don't mistake my grace for weakness.\"" };
                case "Gold Digger":return new string[] { "~g~"+n+":~s~ \"...Fine. You owe me double next time.\"", "~g~"+n+":~s~ \"I'm letting this go. Don't think I'm soft.\"" };
                case "Manipulative":return new string[] { "~g~"+n+":~s~ \"...I'll let it go. You'll owe me for this.\"", "~g~"+n+":~s~ \"Consider this a favor. I don't forget favors.\"" };
                case "Street Smart":return new string[] { "~g~"+n+":~s~ \"Alright, I'll let it slide. Once. Don't test me.\"", "~g~"+n+":~s~ \"You owe me. Remember that.\"" };
                case "Mysterious": return new string[] { "~g~"+n+":~s~ \"...Fine. Just this once.\"", "~g~"+n+":~s~ \"Don't read into it. Just have the money next time.\"" };
                case "Jealous":    return new string[] { "~g~"+n+":~s~ \"I can't believe I'm doing this. Fine.\"", "~g~"+n+":~s~ \"You owe me. Big time.\"" };
                case "Dominant":   return new string[] { "~g~"+n+":~s~ \"I'll allow it. This once. Don't forget you owe me.\"", "~g~"+n+":~s~ \"Consider it a warning. Not a gift.\"" };
                case "Aggressive": return new string[] { "~g~"+n+":~s~ \"...Fine. But don't make me regret it.\"", "~g~"+n+":~s~ \"I'll let it slide. This time.\"" };
                case "Chaotic":    return new string[] { "~g~"+n+":~s~ \"Ha! Fine, whatever. Just this once.\"", "~g~"+n+":~s~ \"You owe me. Or maybe not. I'll decide later.\"" };
                case "Unstable":   return new string[] { "~g~"+n+":~s~ \"Fine! FINE. Just — just have the money next time!\"", "~g~"+n+":~s~ \"I'll let it go. Don't do that again.\"" };
                default:           return new string[] { "~g~"+n+":~s~ \"...Fine. Just this once. Don't make a habit of it.\"", "~g~"+n+":~s~ \"I'll let it slide this time. But you owe me.\"" };
            }
        }

        /// <summary>Personality-matched angry lines when client can't pay in Prostitution A-Life mode.</summary>
        private static string[] GetBrokeProstAngryLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":        return new string[] { "~r~"+n+":~s~ \"You... you don't have the money?\"", "~r~"+n+":~s~ \"Please just go. I'm done.\"" };
                case "Sweet":      return new string[] { "~r~"+n+":~s~ \"You agreed to pay. Where is it?\"", "~r~"+n+":~s~ \"Don't come back if you can't afford it. I'm serious.\"" };
                case "Romantic":   return new string[] { "~r~"+n+":~s~ \"I trusted you and you can't even pay me.\"", "~r~"+n+":~s~ \"This isn't okay. Don't come back.\"" };
                case "Needy":      return new string[] { "~r~"+n+":~s~ \"Why don't you have the money? You promised.\"", "~r~"+n+":~s~ \"Don't do this to me. Pay me or go.\"" };
                case "Flirty":     return new string[] { "~r~"+n+":~s~ \"Are you serious? No money?\"", "~r~"+n+":~s~ \"Come back when you can actually afford it.\"" };
                case "Playful":    return new string[] { "~r~"+n+":~s~ \"Ha! No money. Not funny. Get out.\"", "~r~"+n+":~s~ \"Come back when you're loaded, babe.\"" };
                case "Party Girl": return new string[] { "~r~"+n+":~s~ \"What?! You don't have it?! Come on!\"", "~r~"+n+":~s~ \"Don't waste my time if you can't pay. Get lost.\"" };
                case "Sarcastic":  return new string[] { "~r~"+n+":~s~ \"Oh WOW. You agreed to pay and you're broke. Stunning.\"", "~r~"+n+":~s~ \"Come back when your wallet isn't empty.\"" };
                case "Cold":       return new string[] { "~r~"+n+":~s~ \"No money, no deal. Get out.\"", "~r~"+n+":~s~ \"Don't come back if you can't afford it.\"" };
                case "Independent":return new string[] { "~r~"+n+":~s~ \"You agreed. Where's the cash?\"", "~r~"+n+":~s~ \"I don't work for free. Don't come back without it.\"" };
                case "Classy":     return new string[] { "~r~"+n+":~s~ \"You made a commitment and you can't keep it. Unacceptable.\"", "~r~"+n+":~s~ \"Come back with the funds or don't come back at all.\"" };
                case "Gold Digger":return new string[] { "~r~"+n+":~s~ \"NO money?! Do you know who you're dealing with?\"", "~r~"+n+":~s~ \"Don't waste my time. Get the cash first.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"You agreed to pay. Fascinating that you thought you could get away with this.\"", "~r~"+n+":~s~ \"No money, no deal. Get out.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"You're light. Where's the rest?\"", "~r~"+n+":~s~ \"Don't come to me broke. Get the cash first.\"" };
                case "Mysterious": return new string[] { "~r~"+n+":~s~ \"You said you'd pay. You don't have it. Disappointing.\"", "~r~"+n+":~s~ \"Come back when you do.\"" };
                case "Jealous":    return new string[] { "~r~"+n+":~s~ \"You SAID you had it! What happened?!\"", "~r~"+n+":~s~ \"Don't come back if you can't afford me.\"" };
                case "Dominant":   return new string[] { "~r~"+n+":~s~ \"You agreed to pay. That was the deal. Where is it?\"", "~r~"+n+":~s~ \"Don't come back without the full amount.\"" };
                case "Aggressive": return new string[] { "~r~"+n+":~s~ \"Are you serious? No money? Get out.\"", "~r~"+n+":~s~ \"No money, no deal. Get lost.\"" };
                case "Chaotic":    return new string[] { "~r~"+n+":~s~ \"Ha! No cash! Unbelievable. Get out.\"", "~r~"+n+":~s~ \"Come back when you actually have it.\"" };
                case "Unstable":   return new string[] { "~r~"+n+":~s~ \"You don't have it?! You PROMISED!\"", "~r~"+n+":~s~ \"GET OUT. Come back with the money or don't come back!\"" };
                default:           return new string[] { "~r~"+n+":~s~ \"Are you serious? You don't have the money?\"", "~r~"+n+":~s~ \"No money, no deal. Get lost.\"" };
            }
        }

        /// <summary>Personality-matched accept line for a vehicle invite.</summary>
        private static string GetVehicleInviteAcceptLine(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return "~g~"+n+":~s~ \"O-okay... sure. I'll get in.\"";
                case "Sweet":       return "~g~"+n+":~s~ \"Aw, really? Sure, I'd love that.\"";
                case "Romantic":    return "~g~"+n+":~s~ \"A drive with you? I'll take it.\"";
                case "Needy":       return "~g~"+n+":~s~ \"Yes! I was hoping you'd ask.\"";
                case "Flirty":      return "~g~"+n+":~s~ \"Yeah, I'm in. Let's go.\"";
                case "Playful":     return "~g~"+n+":~s~ \"Shotgun! Let's go!\"";
                case "Party Girl":  return "~g~"+n+":~s~ \"Yeah, why not. Let's roll.\"";
                case "Sarcastic":   return "~g~"+n+":~s~ \"Fine. Beats walking.\"";
                case "Cold":        return "~g~"+n+":~s~ \"Alright. Don't make it a thing.\"";
                case "Independent": return "~g~"+n+":~s~ \"Alright. But I'm out whenever I want.\"";
                case "Classy":      return "~g~"+n+":~s~ \"I can do that. Lead the way.\"";
                case "Gold Digger": return "~g~"+n+":~s~ \"Is it a nice car? ...Fine.\"";
                case "Street Smart":return "~g~"+n+":~s~ \"Yeah, I'll roll with you.\"";
                case "Manipulative":return "~g~"+n+":~s~ \"Smart move inviting me. Alright.\"";
                case "Mysterious":  return "~g~"+n+":~s~ \"...Sure. I'll come.\"";
                case "Jealous":     return "~g~"+n+":~s~ \"This better just be with me.\"";
                case "Dominant":    return "~g~"+n+":~s~ \"Fine. I'll ride with you.\"";
                case "Aggressive":  return "~g~"+n+":~s~ \"Fine. Move it.\"";
                case "Chaotic":     return "~g~"+n+":~s~ \"Ooh yes, let's go already!\"";
                case "Unstable":    return "~g~"+n+":~s~ \"YES. Yes! Get in!\"";
                default:            return "~g~"+n+":~s~ \"Sure, I'll hop in.\"";
            }
        }

        /// <summary>Personality-matched reject line for a vehicle invite.</summary>
        private static string GetVehicleInviteRejectLine(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return "~r~"+n+":~s~ \"I... I'd rather stay out here.\"";
                case "Sweet":       return "~r~"+n+":~s~ \"Maybe another time...\"";
                case "Romantic":    return "~r~"+n+":~s~ \"I'm not quite comfortable yet.\"";
                case "Needy":       return "~r~"+n+":~s~ \"I want to but... I'm scared.\"";
                case "Flirty":      return "~r~"+n+":~s~ \"Not right now, babe.\"";
                case "Playful":     return "~r~"+n+":~s~ \"Ehh, not feeling it right now.\"";
                case "Party Girl":  return "~r~"+n+":~s~ \"Maybe later.\"";
                case "Sarcastic":   return "~r~"+n+":~s~ \"Thanks, but I'll pass.\"";
                case "Cold":        return "~r~"+n+":~s~ \"I'll walk.\"";
                case "Independent": return "~r~"+n+":~s~ \"I don't need a ride.\"";
                case "Classy":      return "~r~"+n+":~s~ \"That's not happening.\"";
                case "Gold Digger": return "~r~"+n+":~s~ \"What's in it for me?\"";
                case "Street Smart":return "~r~"+n+":~s~ \"I don't just hop in strange cars.\"";
                case "Manipulative":return "~r~"+n+":~s~ \"I'll decide when the time is right.\"";
                case "Mysterious":  return "~r~"+n+":~s~ \"Not today.\"";
                case "Jealous":     return "~r~"+n+":~s~ \"Who else have you offered that to?\"";
                case "Dominant":    return "~r~"+n+":~s~ \"Not this time.\"";
                case "Aggressive":  return "~r~"+n+":~s~ \"Back off.\"";
                case "Chaotic":     return "~r~"+n+":~s~ \"Nope! Not today!\"";
                case "Unstable":    return "~r~"+n+":~s~ \"No! Don't ask me!\"";
                default:            return "~r~"+n+":~s~ \"No thanks.\"";
            }
        }

        /// <summary>
        /// Classify an amount into a tier based on personality expectations.
        /// Greedy/Classy/Gold Digger girls set the bar higher; Sweet/Needy/Shy girls
        /// are impressed by less.  Returns 0 = small, 1 = medium, 2 = large.
        /// </summary>
        private static int GetGiveMoneyTier(int amount, double greed)
        {
            // Thresholds scale with greed:
            //   greed=0.10 → small < $50,  medium < $500,  large >= $500
            //   greed=0.50 → small < $200, medium < $2000, large >= $2000
            //   greed=0.95 → small < $500, medium < $5000, large >= $5000
            int medThresh  = (int)(100 + greed * 400);    // 100..500
            int highThresh = (int)(500 + greed * 5000);   // 500..5500
            if (amount >= highThresh) return 2; // large
            if (amount >= medThresh)  return 1; // medium
            return 0;                           // small
        }

        /// <summary>Personality-matched accept lines when player gives money. Tier-aware (0=small,1=med,2=large).</summary>
        private string[] GetGiveMoneyAcceptLines(string name, string personality, int amount, int tier)
        {
            string amtStr = "$" + amount.ToString("N0");
            string n = "~g~" + name + "~s~";
            switch (personality)
            {
                case "Shy":
                    if (tier == 0) return new[] { n + ": \"O-oh... " + amtStr + "? That's... thanks.\"",
                                                  n + ": \"" + amtStr + "... you really didn't have to.\"",
                                                  n + ": \"T-that's for me? ...Okay, thank you.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "?! That's... that's really kind of you.\"",
                                                  n + ": \"Oh my... " + amtStr + "? I-I appreciate that so much.\"",
                                                  n + ": \"Y-you're giving me " + amtStr + "? I don't know what to say...\"" };
                    return new[] { n + ": \"" + amtStr + "?! I— that's— oh my god, thank you!\"",
                                   n + ": \"" + amtStr + "?! Are you serious?! I could cry right now...\"",
                                   n + ": \"I've never... " + amtStr + "?! You're the most generous person I've ever met!\"" };
                case "Flirty":
                    if (tier == 0) return new[] { n + ": \"Aww, " + amtStr + "? That's cute. Thanks, baby.\"",
                                                  n + ": \"" + amtStr + "? A little something-something, huh? I'll take it.\"",
                                                  n + ": \"" + amtStr + ". Sweet of you, handsome.\"" };
                    if (tier == 1) return new[] { n + ": \"Mmm, " + amtStr + "? You sure know how to treat a girl.\"",
                                                  n + ": \"" + amtStr + "? Baby, you're making me blush.\"",
                                                  n + ": \"Ooh, " + amtStr + "... I think I like you even more now.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Oh my... daddy's got deep pockets!\"",
                                   n + ": \"" + amtStr + "?! You're spoiling me rotten and I LOVE it.\"",
                                   n + ": \"" + amtStr + "?! Okay, you're officially my favorite person alive.\"" };
                case "Dominant":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + ". It's a start.\"",
                                                  n + ": \"" + amtStr + "? Fine. I'll accept.\"",
                                                  n + ": \"" + amtStr + ". At least you're trying.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + ". Smart move. I'll take it.\"",
                                                  n + ": \"" + amtStr + ". Good. You know how to keep me happy.\"",
                                                  n + ": \"" + amtStr + "? Acceptable. Hand it over.\"" };
                    return new[] { n + ": \"" + amtStr + ". Now THAT'S how you treat a queen.\"",
                                   n + ": \"" + amtStr + "? Good boy. You've earned my attention.\"",
                                   n + ": \"" + amtStr + ". Finally, someone who understands my worth.\"" };
                case "Cold":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + ". ...Fine.\"",
                                                  n + ": \"" + amtStr + ". I suppose I'll take it.\"",
                                                  n + ": \"" + amtStr + ". Whatever.\"" };
                    if (tier == 1) return new[] { n + ": \"Fine. " + amtStr + ". I'll accept it.\"",
                                                  n + ": \"" + amtStr + ". ...Noted.\"",
                                                  n + ": \"I'll take the " + amtStr + ". Don't expect a thank-you card.\"" };
                    return new[] { n + ": \"" + amtStr + ". ...That's actually generous. I'll remember this.\"",
                                   n + ": \"" + amtStr + ". You have my attention now.\"",
                                   n + ": \"" + amtStr + ". I won't pretend I'm not impressed.\"" };
                case "Sweet":
                    if (tier == 0) return new[] { n + ": \"Aww, " + amtStr + "? That's so thoughtful!\"",
                                                  n + ": \"" + amtStr + "? You're so nice! Thank you!\"",
                                                  n + ": \"Even " + amtStr + " makes me happy. It's the thought that counts!\"" };
                    if (tier == 1) return new[] { n + ": \"Oh my gosh, " + amtStr + "? You're so sweet!\"",
                                                  n + ": \"" + amtStr + "?! You're the sweetest person ever!\"",
                                                  n + ": \"Aww, " + amtStr + "? That really makes my day!\"" };
                    return new[] { n + ": \"" + amtStr + "?! Oh my GOD, you're an angel!\"",
                                   n + ": \"" + amtStr + "?! I think I'm gonna cry happy tears!\"",
                                   n + ": \"" + amtStr + "?! You're literally the best person in the world!\"" };
                case "Gold Digger":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? ...I guess it's something.\"",
                                                  n + ": \"" + amtStr + ". Hmm. I've seen bigger.\"",
                                                  n + ": \"" + amtStr + "? Cute. I'll add it to the pile.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? Now that's what I like to hear.\"",
                                                  n + ": \"" + amtStr + ". See? This is why I keep you around.\"",
                                                  n + ": \"" + amtStr + "? Mmm, you're learning. Keep it coming.\"" };
                    return new[] { n + ": \"" + amtStr + "?! NOW we're talking, baby!\"",
                                   n + ": \"" + amtStr + "?! You just became my NUMBER ONE.\"",
                                   n + ": \"" + amtStr + "?! I KNEW you were worth my time! Don't ever stop.\"" };
                case "Street Smart":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? Alright, every bit helps.\"",
                                                  n + ": \"" + amtStr + ". I'll put it to good use.\"",
                                                  n + ": \"" + amtStr + ". No strings? Cool.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? Alright, I'll hold onto that.\"",
                                                  n + ": \"" + amtStr + ". No strings? ...Cool. I'll take it.\"",
                                                  n + ": \"" + amtStr + "? Smart investment on your part.\"" };
                    return new[] { n + ": \"" + amtStr + "? Damn. You're serious, huh? Respect.\"",
                                   n + ": \"" + amtStr + ". That's real money. I won't forget this.\"",
                                   n + ": \"" + amtStr + "? You've got my loyalty now. For real.\"" };
                case "Party Girl":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? Hey, that's a round of drinks!\"",
                                                  n + ": \"" + amtStr + "? Shots shots shots!\"",
                                                  n + ": \"" + amtStr + "? Enough for some fun! Thanks!\"" };
                    if (tier == 1) { bool _nt = IsNight(); return new[] { n + ": \"" + amtStr + "? " + (_nt ? "Drinks are on me tonight!" : "Drinks are on me!") + "\"",
                                                  n + ": \"" + amtStr + "?! Yesss, we're going out!\"",
                                                  n + ": \"Oh hell yeah, " + amtStr + "! The party just got better!\"" }; }
                    return new[] { n + ": \"" + amtStr + "?! VIP BABY! WE'RE GOING ALL NIGHT!\"",
                                   n + ": \"" + amtStr + "?! Oh my GOD this is gonna be LEGENDARY!\"",
                                   n + ": \"" + amtStr + "?! BOTTLE SERVICE! Private jet! I love you!\"" };
                case "Romantic":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? It's the gesture that matters. Thank you.\"",
                                                  n + ": \"" + amtStr + "... That's really sweet of you.\"",
                                                  n + ": \"You thought of me... " + amtStr + " is more than enough.\"" };
                    if (tier == 1) return new[] { n + ": \"You're giving me " + amtStr + "? That means a lot...\"",
                                                  n + ": \"" + amtStr + "... It's not about the money, it's the thought.\"",
                                                  n + ": \"" + amtStr + "? You really do care about me, don't you?\"" };
                    return new[] { n + ": \"" + amtStr + "?! I... I don't even know what to say. You're amazing.\"",
                                   n + ": \"" + amtStr + "?! This is... you make me believe in love again.\"",
                                   n + ": \"" + amtStr + "?! I've never felt so cared for in my entire life...\"" };
                case "Sarcastic":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? Wow, big spender. Sure, I'll take it.\"",
                                                  n + ": \"" + amtStr + ". Try not to go bankrupt. ...Thanks.\"",
                                                  n + ": \"" + amtStr + ". A whole coffee's worth. I'm moved.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? Wow, a gentleman. I'm shocked.\"",
                                                  n + ": \"" + amtStr + "? What is this, a Hallmark movie? ...Thanks.\"",
                                                  n + ": \"Oh, " + amtStr + ". My hero. I'll try not to cry.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Okay, I'll admit it — I'm actually impressed. Don't tell anyone.\"",
                                   n + ": \"" + amtStr + "?! Did you rob a bank or are you just naturally this stupid-generous?\"",
                                   n + ": \"" + amtStr + "?! ...Okay, fine, you win. I like you. Happy?\"" };
                case "Needy":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? You really thought of me? Thank you...\"",
                                                  n + ": \"Even " + amtStr + " means so much coming from you.\"",
                                                  n + ": \"" + amtStr + "... You're so good to me.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? Thank you so much, I really needed that.\"",
                                                  n + ": \"" + amtStr + "?! Oh, you have no idea how much this helps.\"",
                                                  n + ": \"You're giving me " + amtStr + "? I... I really appreciate that.\"" };
                    return new[] { n + ": \"" + amtStr + "?! I— you— please never leave me!\"",
                                   n + ": \"" + amtStr + "?! I've never had anyone care about me this much!\"",
                                   n + ": \"" + amtStr + "?! You're the only person who's ever been there for me!\"" };
                case "Independent":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + ". I don't need it, but fine.\"",
                                                  n + ": \"" + amtStr + ". ...Sure. Whatever.\"",
                                                  n + ": \"" + amtStr + ". Noted. Don't make it a habit.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + ". I don't need it, but... thanks.\"",
                                                  n + ": \"" + amtStr + "? I can take care of myself, but... okay.\"",
                                                  n + ": \"" + amtStr + ". Don't think this means I owe you anything.\"" };
                    return new[] { n + ": \"" + amtStr + "?! That's... okay, I respect the gesture.\"",
                                   n + ": \"" + amtStr + "?! I'm not gonna lie, that's impressive.\"",
                                   n + ": \"" + amtStr + "?! ...Fine. You've earned my respect. Genuinely.\"" };
                case "Jealous":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? Just for me, right?\"",
                                                  n + ": \"" + amtStr + ". This better be exclusive.\"",
                                                  n + ": \"" + amtStr + "... You don't give money to other girls, do you?\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? Better not be giving this to anyone else.\"",
                                                  n + ": \"" + amtStr + "... This is just for me, right?\"",
                                                  n + ": \"I'll take the " + amtStr + ". But only if I'm your only girl.\"" };
                    return new[] { n + ": \"" + amtStr + "?! You better not give ANYONE else this kind of money!\"",
                                   n + ": \"" + amtStr + "?! I'm the ONLY one who gets this, right?! RIGHT?!\"",
                                   n + ": \"" + amtStr + "?! That proves I'm special to you! ...I AM, right?!\"" };
                case "Chaotic":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? Heh. I'll find a way to make this interesting.\"",
                                                  n + ": \"" + amtStr + "? Chaos fund! Let's go!\"",
                                                  n + ": \"" + amtStr + ". Pocket change, but it's the thought that counts. Maybe.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "?! Ha! You're crazy. I love it.\"",
                                                  n + ": \"" + amtStr + "?! Hahaha, you're insane! Gimme!\"",
                                                  n + ": \"" + amtStr + "?! Oh this is gonna be FUN.\"" };
                    { bool _nt = IsNight(); return new[] { n + ": \"" + amtStr + "?! HAHAHAHA! " + (_nt ? "We're burning this city DOWN tonight!" : "We're burning this city DOWN!") + "\"",
                                   n + ": \"" + amtStr + "?! You absolute MANIAC! I'm in LOVE!\"",
                                   n + ": \"" + amtStr + "?! The chaos we're gonna cause with this... *chef's kiss*\"" }; }
                case "Manipulative":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? Hmm. I appreciate the effort.\"",
                                                  n + ": \"" + amtStr + ". A token. But I see the intention behind it.\"",
                                                  n + ": \"" + amtStr + ". You're trying. That's... something.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "... I knew you'd come through for me.\"",
                                                  n + ": \"" + amtStr + ". See? I always knew you were the generous type.\"",
                                                  n + ": \"" + amtStr + "... You're such a good person for doing this.\"" };
                    return new[] { n + ": \"" + amtStr + "?! I always knew you were my most reliable investment.\"",
                                   n + ": \"" + amtStr + "?! You've exceeded expectations. I'll have to reward you... later.\"",
                                   n + ": \"" + amtStr + "?! You've proven yourself completely. I'm... genuinely touched.\"" };
                case "Aggressive":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + ". Better than nothing.\"",
                                                  n + ": \"" + amtStr + ". Hmph. Fine.\"",
                                                  n + ": \"" + amtStr + ". Don't act like you're doing me a favor.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + ". Took you long enough.\"",
                                                  n + ": \"" + amtStr + ". About damn time.\"",
                                                  n + ": \"" + amtStr + "? Good. Now we're talking.\"" };
                    return new[] { n + ": \"" + amtStr + "?! ...Alright, I respect that. Seriously.\"",
                                   n + ": \"" + amtStr + "?! Okay... okay, you're not as useless as I thought.\"",
                                   n + ": \"" + amtStr + "?! Damn. You actually came through. ...Thanks.\"" };
                case "Playful":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? Yay, candy money! Thanks!\"",
                                                  n + ": \"" + amtStr + "? Ooh, gumball fund!\"",
                                                  n + ": \"" + amtStr + "? That's adorable. I'll take it!\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? Ooh, is this a bribe? I accept!\"",
                                                  n + ": \"" + amtStr + "?! You're like my personal ATM! Kidding... mostly.\"",
                                                  n + ": \"Ooh " + amtStr + "! Can I spend it all on candy?\"" };
                    return new[] { n + ": \"" + amtStr + "?! JACKPOT! I'm doing a happy dance RIGHT NOW!\"",
                                   n + ": \"" + amtStr + "?! Can I adopt you?! You're the best!\"",
                                   n + ": \"" + amtStr + "?! *gasp* We're going to Disneyland!\"" };
                case "Mysterious":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + ". ...I'll accept.\"",
                                                  n + ": \"" + amtStr + ". A small gesture. Noted.\"",
                                                  n + ": \"" + amtStr + ". ...Curious.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "... Interesting. I'll keep it close.\"",
                                                  n + ": \"" + amtStr + "... How generous. I won't forget this.\"",
                                                  n + ": \"" + amtStr + "... You're full of surprises.\"" };
                    return new[] { n + ": \"" + amtStr + "... You continue to fascinate me.\"",
                                   n + ": \"" + amtStr + "... Few people have ever surprised me like this.\"",
                                   n + ": \"" + amtStr + "... Perhaps you're more than you seem.\"" };
                case "Classy":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + ". A modest gesture. I accept.\"",
                                                  n + ": \"" + amtStr + ". It's the etiquette that matters. Thank you.\"",
                                                  n + ": \"" + amtStr + ". How considerate.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + ". How generous of you. Thank you.\"",
                                                  n + ": \"" + amtStr + ". A gracious gesture. I'm impressed.\"",
                                                  n + ": \"" + amtStr + "? Quite the gentleman. I appreciate it.\"" };
                    return new[] { n + ": \"" + amtStr + "?! My word... you are extraordinarily generous.\"",
                                   n + ": \"" + amtStr + "?! I must say, I am genuinely moved. Thank you.\"",
                                   n + ": \"" + amtStr + "?! A person of true class. I'm deeply impressed.\"" };
                case "Unstable":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? Okay! Wait, should I? Yes! Maybe? YES.\"",
                                                  n + ": \"" + amtStr + "? *takes it* *gives it back* *takes it again* MINE.\"",
                                                  n + ": \"" + amtStr + "! I love it! Wait, do I? YES. Thanks.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "?! Oh my god YES. Wait, why? Whatever, gimme.\"",
                                                  n + ": \"" + amtStr + "?! I love you! No wait, I don't. But GIMME.\"",
                                                  n + ": \"" + amtStr + "?! *laughs* *cries* *takes it* Thank you!\"" };
                    return new[] { n + ": \"" + amtStr + "?! AHHHHH! I'm laughing AND crying! Is this real?!\"",
                                   n + ": \"" + amtStr + "?! I WILL DIE FOR YOU! Wait no— I LIVE for you! SAME THING!\"",
                                   n + ": \"" + amtStr + "?! *screams* *hugs you* *pushes you* *hugs again* THANK YOU!\"" };
                default:
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? Sure, thanks.\"",
                                                  n + ": \"" + amtStr + ". I'll take it.\"",
                                                  n + ": \"" + amtStr + ". That's nice.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? Thanks, I appreciate that.\"",
                                                  n + ": \"" + amtStr + "? That's generous, thanks.\"",
                                                  n + ": \"" + amtStr + "? Cool, thank you.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Wow, thank you so much!\"",
                                   n + ": \"" + amtStr + "?! That's amazing, thank you!\"",
                                   n + ": \"" + amtStr + "?! I really don't know what to say. Thanks!\"" };
            }
        }

        /// <summary>Personality-matched reject lines when player gives money. Tier-aware (0=small,1=med,2=large).</summary>
        private string[] GetGiveMoneyRejectLines(string name, string personality, int amount, int tier)
        {
            string amtStr = "$" + amount.ToString("N0");
            string n = "~r~" + name + "~s~";
            switch (personality)
            {
                case "Shy":
                    if (tier == 0) return new[] { n + ": \"I... I can't take that. Sorry.\"",
                                                  n + ": \"N-no, it's okay... keep it.\"",
                                                  n + ": \"I-I appreciate it, but no...\"" };
                    if (tier == 1) return new[] { n + ": \"That's... that's too much. I can't.\"",
                                                  n + ": \"N-no, I couldn't... please keep it.\"",
                                                  n + ": \"I'd feel weird taking " + amtStr + "...\"" };
                    return new[] { n + ": \"" + amtStr + "?! N-no! That's way too much! Please!\"",
                                   n + ": \"I can't— " + amtStr + "?! That's insane, I could never!\"",
                                   n + ": \"" + amtStr + "?! Oh my god, no! I'd feel awful!\"" };
                case "Flirty":
                    if (tier == 0) return new[] { n + ": \"Cute, but I'm not taking your money, baby.\"",
                                                  n + ": \"" + amtStr + "? Honey, buy me a drink instead.\"",
                                                  n + ": \"Save that for a date, handsome.\"" };
                    if (tier == 1) return new[] { n + ": \"Honey, I don't need your " + amtStr + ". Just your attention.\"",
                                                  n + ": \"Save your money, handsome. Buy me dinner instead.\"",
                                                  n + ": \"" + amtStr + "? I'm flattered, but no.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Baby, you're sweet but I can't accept that.\"",
                                   n + ": \"" + amtStr + "?! As much as I'd love to... no. Not like this.\"",
                                   n + ": \"" + amtStr + "?! You're trying too hard, gorgeous. Relax.\"" };
                case "Dominant":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? Please. Don't insult me.\"",
                                                  n + ": \"" + amtStr + ". Is that supposed to impress me?\"",
                                                  n + ": \"" + amtStr + ". I'm not your charity case.\"" };
                    if (tier == 1) return new[] { n + ": \"Keep your " + amtStr + ". I don't need handouts.\"",
                                                  n + ": \"" + amtStr + "? I'm not your charity case.\"",
                                                  n + ": \"Put that " + amtStr + " away. I don't take orders or money.\"" };
                    return new[] { n + ": \"" + amtStr + "?! You can't buy my respect. Earn it.\"",
                                   n + ": \"" + amtStr + "?! Impressive, but I don't accept money. Period.\"",
                                   n + ": \"" + amtStr + "?! I don't need your money. I need your obedience.\"" };
                case "Cold":
                    if (tier == 0) return new[] { n + ": \"No.\"",
                                                  n + ": \"" + amtStr + ". Pointless. No.\"",
                                                  n + ": \"I didn't ask for that.\"" };
                    if (tier == 1) return new[] { n + ": \"No. I'm not interested in your money.\"",
                                                  n + ": \"" + amtStr + "? No. End of discussion.\"",
                                                  n + ": \"I didn't ask for your money. Don't offer again.\"" };
                    return new[] { n + ": \"" + amtStr + "? The answer is still no. It will always be no.\"",
                                   n + ": \"" + amtStr + ". You think money changes things? It doesn't.\"",
                                   n + ": \"" + amtStr + ". Save it. I'm not for sale.\"" };
                case "Sweet":
                    if (tier == 0) return new[] { n + ": \"Aww, that's sweet, but I'm okay!\"",
                                                  n + ": \"You're so kind, but really, keep it!\"",
                                                  n + ": \"That's thoughtful, but I couldn't take it.\"" };
                    if (tier == 1) return new[] { n + ": \"That's really nice, but I can't accept it.\"",
                                                  n + ": \"Aww, you're so kind, but no... I couldn't.\"",
                                                  n + ": \"I appreciate the thought, but it wouldn't feel right.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Oh no no no, that's WAY too generous! I can't!\"",
                                   n + ": \"" + amtStr + "?! You're the sweetest, but I'd feel so guilty!\"",
                                   n + ": \"" + amtStr + "?! Please, that's too much! Just your company is enough!\"" };
                case "Gold Digger":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? That's it? Don't waste my time.\"",
                                                  n + ": \"" + amtStr + "? Ha! Come back with real money.\"",
                                                  n + ": \"" + amtStr + "? I have standards, honey.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? Hmph. Come back when you're serious.\"",
                                                  n + ": \"" + amtStr + "? That's not gonna cut it.\"",
                                                  n + ": \"I have standards, and " + amtStr + " ain't meeting them.\"" };
                    return new[] { n + ": \"" + amtStr + "? Hmm... tempting, but I'm not in the mood.\"",
                                   n + ": \"" + amtStr + "? ...Ask me again later. Maybe.\"",
                                   n + ": \"" + amtStr + ". Not today. But don't give up.\"" };
                case "Street Smart":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? What's the catch?\"",
                                                  n + ": \"Nothing's free. I'll pass.\"",
                                                  n + ": \"I don't take money unless I know the angle.\"" };
                    if (tier == 1) return new[] { n + ": \"Free money? Nothing's free. I'll pass.\"",
                                                  n + ": \"" + amtStr + "? What's the catch? Nah, I'm good.\"",
                                                  n + ": \"I don't take money I didn't earn. Keep it.\"" };
                    return new[] { n + ": \"" + amtStr + "? That's a LOT of strings I don't want attached.\"",
                                   n + ": \"" + amtStr + "? Nobody gives that kind of money for free. Pass.\"",
                                   n + ": \"" + amtStr + "? My gut says no. And I always trust my gut.\"" };
                case "Party Girl":
                    if (tier == 0) return new[] { n + ": \"Nah, I'm here for fun, not money.\"",
                                                  n + ": \"Keep it! Let's just have a good time!\"",
                                                  n + ": \"Money's boring! Let's party instead!\"" };
                    if (tier == 1) return new[] { n + ": \"Nah, I don't need your money for a good time.\"",
                                                  n + ": \"Keep your " + amtStr + ", I'm here for the vibes!\"",
                                                  n + ": \"Money's boring! Buy me shots or nothing.\"" };
                    return new[] { n + ": \"" + amtStr + "?! That's crazy but I can't! Just party with me!\"",
                                   n + ": \"" + amtStr + "?! Nooo, spend it ON the party, not ON me!\"",
                                   n + ": \"" + amtStr + "?! I don't want your money, I want your energy!\"" };
                case "Romantic":
                    if (tier == 0) return new[] { n + ": \"That's sweet, but I don't need money.\"",
                                                  n + ": \"I'd rather have your time than your money.\"",
                                                  n + ": \"" + amtStr + " can't buy what matters. Keep it.\"" };
                    if (tier == 1) return new[] { n + ": \"I don't want your money... I want something real.\"",
                                                  n + ": \"" + amtStr + " won't buy what I'm looking for.\"",
                                                  n + ": \"You can't put a price on what matters. Keep it.\"" };
                    return new[] { n + ": \"" + amtStr + "?! That's... no. Love isn't a transaction.\"",
                                   n + ": \"" + amtStr + "?! I appreciate it, but this isn't how I want us to be.\"",
                                   n + ": \"" + amtStr + "?! Please... just be here with me. That's enough.\"" };
                case "Sarcastic":
                    if (tier == 0) return new[] { n + ": \"" + amtStr + "? What am I, a tip jar?\"",
                                                  n + ": \"" + amtStr + ". Wow. I'm overwhelmed. Not.\"",
                                                  n + ": \"" + amtStr + "? Gee thanks, Mr. Money Bags.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? What am I, a tip jar? No thanks.\"",
                                                  n + ": \"Oh wow, " + amtStr + ". Should I curtsy? ...No.\"",
                                                  n + ": \"" + amtStr + "? Is this a joke? Because I'm not laughing.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Wow, are you proposing or bribing me? Either way, no.\"",
                                   n + ": \"" + amtStr + "?! That's a lot of money for a 'no.' And it's still a no.\"",
                                   n + ": \"" + amtStr + "?! I'm flattered, truly. But absolutely not.\"" };
                case "Needy":
                    if (tier == 0) return new[] { n + ": \"I... no. I don't want to owe you.\"",
                                                  n + ": \"If I take it, you'll expect something...\"",
                                                  n + ": \"Please don't... it makes me feel weird.\"" };
                    if (tier == 1) return new[] { n + ": \"I... no. I can't owe you anything.\"",
                                                  n + ": \"If I take it, you'll expect something... I can't.\"",
                                                  n + ": \"Please don't... I don't want to feel like I need anyone.\"" };
                    return new[] { n + ": \"" + amtStr + "?! N-no! That's too much responsibility!\"",
                                   n + ": \"" + amtStr + "?! I'd panic every day worrying I owe you!\"",
                                   n + ": \"" + amtStr + "?! I can't handle that kind of... no! Please!\"" };
                case "Independent":
                    if (tier == 0) return new[] { n + ": \"I take care of myself. Keep it.\"",
                                                  n + ": \"I'm good. Don't need it.\"",
                                                  n + ": \"Thanks, but I earn my own way.\"" };
                    if (tier == 1) return new[] { n + ": \"I don't need " + amtStr + " from anyone.\"",
                                                  n + ": \"Thanks, but I earn my own money.\"",
                                                  n + ": \"I'm not a charity. Keep it.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Absolutely not. I don't need anyone's money.\"",
                                   n + ": \"" + amtStr + "?! I'm not for sale, and I'm not a project.\"",
                                   n + ": \"" + amtStr + "?! I respect the gesture, but I stand on my own.\"" };
                case "Jealous":
                    if (tier == 0) return new[] { n + ": \"Who else did you offer money to?\"",
                                                  n + ": \"" + amtStr + "? Are you giving this to other girls too?\"",
                                                  n + ": \"I don't want money. I want your attention.\"" };
                    if (tier == 1) return new[] { n + ": \"Are you trying to buy my trust? It doesn't work like that.\"",
                                                  n + ": \"" + amtStr + "? Who else are you throwing money at?\"",
                                                  n + ": \"I don't want your money. I want your loyalty.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Is this guilt money?! What did you DO?!\"",
                                   n + ": \"" + amtStr + "?! You're DEFINITELY hiding something! Who is she?!\"",
                                   n + ": \"" + amtStr + "?! No amount of money replaces LOYALTY!\"" };
                case "Chaotic":
                    if (tier == 0) return new[] { n + ": \"Money? Boring. Do something exciting.\"",
                                                  n + ": \"" + amtStr + "? Predictable. Next.\"",
                                                  n + ": \"Cash is lame. Entertain me instead.\"" };
                    if (tier == 1) return new[] { n + ": \"Money? Boring. I wanted entertainment.\"",
                                                  n + ": \"" + amtStr + "? Ugh, so predictable. No.\"",
                                                  n + ": \"Cash is lame! Do something crazy instead.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Hahaha NO! What would I even DO with that?! Actually...no.\"",
                                   n + ": \"" + amtStr + "?! You think MONEY can contain THIS energy?! HA!\"",
                                   n + ": \"" + amtStr + "?! Tempting but NO! I refuse to be predictable!\"" };
                case "Manipulative":
                    if (tier == 0) return new[] { n + ": \"Not now. I'll let you know when I need something.\"",
                                                  n + ": \"" + amtStr + "? Save it. The timing isn't right.\"",
                                                  n + ": \"Hold onto that for now. I have plans.\"" };
                    if (tier == 1) return new[] { n + ": \"Not now. Maybe when I actually need something.\"",
                                                  n + ": \"" + amtStr + "? Hmm... not the right time. Save it.\"",
                                                  n + ": \"Hold onto that. I'll let you know when I want it.\"" };
                    return new[] { n + ": \"" + amtStr + "? Impressive... but I prefer to keep you in debt to me.\"",
                                   n + ": \"" + amtStr + "? No. I'd rather you owed me a favor instead.\"",
                                   n + ": \"" + amtStr + "? I don't want money. I want leverage. Keep it.\"" };
                case "Aggressive":
                    if (tier == 0) return new[] { n + ": \"I said no.\"",
                                                  n + ": \"" + amtStr + "? Get that out of my face.\"",
                                                  n + ": \"Did I stutter? No.\"" };
                    if (tier == 1) return new[] { n + ": \"I said no. Don't push it.\"",
                                                  n + ": \"" + amtStr + "? Shove it. I don't want your money.\"",
                                                  n + ": \"Keep waving that " + amtStr + " around and see what happens.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Are you trying to buy me?! BAD idea.\"",
                                   n + ": \"" + amtStr + "?! I will BREAK you if you try that again.\"",
                                   n + ": \"" + amtStr + "?! You think money makes you untouchable?! Think again.\"" };
                case "Playful":
                    if (tier == 0) return new[] { n + ": \"Ha! Nice try. I'm not that easy.\"",
                                                  n + ": \"Nope! But cute attempt, hehe.\"",
                                                  n + ": \"" + amtStr + "? Pfft, buy me ice cream instead.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? Pfft, you can't buy this much fun.\"",
                                                  n + ": \"Nope! But A for effort, hehe.\"",
                                                  n + ": \"Ha! Nice try. I'm not that easy.\"" };
                    return new[] { n + ": \"" + amtStr + "?! Omg hahaha NO! But that was hilarious to see!\"",
                                   n + ": \"" + amtStr + "?! You're CRAZY! And I love it! But still no!\"",
                                   n + ": \"" + amtStr + "?! I can't be bought! ...But I CAN be bribed with tacos.\"" };
                case "Mysterious":
                    if (tier == 0) return new[] { n + ": \"...No.\"",
                                                  n + ": \"" + amtStr + ". Not interested.\"",
                                                  n + ": \"Some things aren't about money.\"" };
                    if (tier == 1) return new[] { n + ": \"...No. Some things can't be bought.\"",
                                                  n + ": \"" + amtStr + "... Tempting. But no.\"",
                                                  n + ": \"I have my reasons for refusing. Leave it at that.\"" };
                    return new[] { n + ": \"" + amtStr + "... The answer remains the same. No.\"",
                                   n + ": \"" + amtStr + "... You'd understand my refusal if you knew more.\"",
                                   n + ": \"" + amtStr + "... Money can't buy what I'm protecting.\"" };
                case "Classy":
                    if (tier == 0) return new[] { n + ": \"I appreciate the thought, but no.\"",
                                                  n + ": \"A lady doesn't accept pocket change. No offense.\"",
                                                  n + ": \"That's kind, but I must decline.\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "? That's generous, but I must decline.\"",
                                                  n + ": \"A lady doesn't accept money from strangers. No offense.\"",
                                                  n + ": \"I appreciate the gesture, but no.\"" };
                    return new[] { n + ": \"" + amtStr + "? Extraordinarily generous, but a lady has principles.\"",
                                   n + ": \"" + amtStr + "? I'm deeply touched, but I couldn't accept.\"",
                                   n + ": \"" + amtStr + "? That speaks well of you, but I must respectfully decline.\"" };
                case "Unstable":
                    if (tier == 0) return new[] { n + ": \"No! Wait— yes! NO! I said no!\"",
                                                  n + ": \"" + amtStr + "?! Ugh! My brain hurts! NO!\"",
                                                  n + ": \"Don't— just— UGH! NO!\"" };
                    if (tier == 1) return new[] { n + ": \"" + amtStr + "?! No! I don't— just— no!\"",
                                                  n + ": \"NO! Wait— no! I can't— UGH!\"",
                                                  n + ": \"" + amtStr + "?! My brain says yes but my mouth says NO!\"" };
                    return new[] { n + ": \"" + amtStr + "?! AHHH! YES! NO! *slaps self* NO! FINAL ANSWER! MAYBE! NO!\"",
                                   n + ": \"" + amtStr + "?! I HATE that I want to say yes! BUT NO! PROBABLY! NO!\"",
                                   n + ": \"" + amtStr + "?! *screams internally* *screams externally* STILL NO!\"" };
                default:
                    if (tier == 0) return new[] { n + ": \"No thanks.\"",
                                                  n + ": \"I'd rather not.\"",
                                                  n + ": \"I'm good, thanks.\"" };
                    if (tier == 1) return new[] { n + ": \"No, but thanks for the offer.\"",
                                                  n + ": \"I appreciate it, but no.\"",
                                                  n + ": \"That's kind, but I'll pass.\"" };
                    return new[] { n + ": \"" + amtStr + "? That's generous, but no.\"",
                                   n + ": \"" + amtStr + "? I really can't accept that.\"",
                                   n + ": \"" + amtStr + "? Wow, but... no. Thanks though.\"" };
            }
        }

        private static string[] GetArrangementEndAngryLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~r~"+n+":~s~ \"I can't believe you'd just... end it like that.\"", "~r~"+n+":~s~ \"That really hurt. I thought we had something.\"", "~r~"+n+":~s~ \"You're just cutting me off? Just like that?\"", "~r~"+n+":~s~ \"I don't understand. Why are you doing this?\"" };
                case "Sweet":       return new string[] { "~r~"+n+":~s~ \"I gave you everything and this is what I get?\"", "~r~"+n+":~s~ \"After all I've done for you? That's how you end it?\"", "~r~"+n+":~s~ \"I was good to you. You didn't deserve that.\"", "~r~"+n+":~s~ \"That's really not okay. I'm hurt.\"" };
                case "Romantic":    return new string[] { "~r~"+n+":~s~ \"You're ending what we had? Just like that?\"", "~r~"+n+":~s~ \"This was supposed to mean something. You ruined it.\"", "~r~"+n+":~s~ \"I can't believe you'd throw this away.\"", "~r~"+n+":~s~ \"Don't. Just... don't do this.\"" };
                case "Needy":       return new string[] { "~r~"+n+":~s~ \"No no no. You can't leave. I need this.\"", "~r~"+n+":~s~ \"Please don't do this. I'm begging you.\"", "~r~"+n+":~s~ \"You're all I had and now you're doing this?\"", "~r~"+n+":~s~ \"Why?! What did I do wrong?!\"" };
                case "Flirty":      return new string[] { "~r~"+n+":~s~ \"Wow. Really? After all the fun we had?\"", "~r~"+n+":~s~ \"You're seriously ending this right now? Cold.\"", "~r~"+n+":~s~ \"I didn't see that coming. That stings.\"", "~r~"+n+":~s~ \"And here I thought you liked me.\"" };
                case "Playful":     return new string[] { "~r~"+n+":~s~ \"Are you being serious right now? Not funny.\"", "~r~"+n+":~s~ \"Ha. Oh wait, you're actually serious? Ouch.\"", "~r~"+n+":~s~ \"So that's just... it? Wow.\"", "~r~"+n+":~s~ \"Not the ending I expected. Thanks for nothing.\"" };
                case "Party Girl":  return new string[] { "~r~"+n+":~s~ \"Seriously?! We had a good thing going!\"", "~r~"+n+":~s~ \"You're ruining everything. This was fun!\"", "~r~"+n+":~s~ \"I can't believe you're doing this right now.\"", "~r~"+n+":~s~ \"That's messed up. I'm out.\"" };
                case "Sarcastic":   return new string[] { "~r~"+n+":~s~ \"Oh, wow. Groundbreaking. Cutting me off. Original.\"", "~r~"+n+":~s~ \"Sure. End it. Real mature move there.\"", "~r~"+n+":~s~ \"Great. Thanks for that. Very helpful.\"", "~r~"+n+":~s~ \"And here I thought you had a spine.\"" };
                case "Cold":        return new string[] { "~r~"+n+":~s~ \"Fine. Don't expect me to care.\"", "~r~"+n+":~s~ \"Walk away then. I already moved on.\"", "~r~"+n+":~s~ \"You think I'm surprised? I'm not.\"", "~r~"+n+":~s~ \"Done. Just like that. Typical.\"" };
                case "Independent": return new string[] { "~r~"+n+":~s~ \"I didn't need this anyway.\"", "~r~"+n+":~s~ \"Fine. I'll figure it out myself. I always do.\"", "~r~"+n+":~s~ \"You cutting me off doesn't change anything for me.\"", "~r~"+n+":~s~ \"I wasn't relying on you anyway.\"" };
                case "Classy":      return new string[] { "~r~"+n+":~s~ \"How disappointingly abrupt. I expected better.\"", "~r~"+n+":~s~ \"You could've at least been civil about it.\"", "~r~"+n+":~s~ \"I find this entire situation beneath me.\"", "~r~"+n+":~s~ \"Consider this bridge thoroughly burned.\"" };
                case "Gold Digger": return new string[] { "~r~"+n+":~s~ \"Are you serious?! Do you know what you're costing me?\"", "~r~"+n+":~s~ \"You're throwing away a paying arrangement? Idiot.\"", "~r~"+n+":~s~ \"I can't believe you just cut off my income like that.\"", "~r~"+n+":~s~ \"You're going to regret losing this.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"You'll regret this. Mark my words.\"", "~r~"+n+":~s~ \"That was a very stupid move on your part.\"", "~r~"+n+":~s~ \"Fine. But don't come crawling back.\"", "~r~"+n+":~s~ \"I gave you a chance and you threw it away.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"You're cutting me off? Big mistake.\"", "~r~"+n+":~s~ \"I don't forget this kind of thing.\"", "~r~"+n+":~s~ \"You have some nerve doing that to me.\"", "~r~"+n+":~s~ \"Watch yourself. I remember faces.\"" };
                case "Mysterious":  return new string[] { "~r~"+n+":~s~ \"...I didn't expect this.\"", "~r~"+n+":~s~ \"You ending it changes more than you know.\"", "~r~"+n+":~s~ \"I see. Good luck with that.\"", "~r~"+n+":~s~ \"You'll wonder about this decision later.\"" };
                case "Jealous":     return new string[] { "~r~"+n+":~s~ \"Is there someone else? Don't lie to me.\"", "~r~"+n+":~s~ \"You're doing this because of someone else, aren't you?\"", "~r~"+n+":~s~ \"I knew it. I knew you'd do this.\"", "~r~"+n+":~s~ \"After everything? You're cutting ME off?\"" };
                case "Dominant":    return new string[] { "~r~"+n+":~s~ \"You don't end this. I do. Get that straight.\"", "~r~"+n+":~s~ \"Excuse me? You think you're in charge of that?\"", "~r~"+n+":~s~ \"Nobody cuts me off. Nobody.\"", "~r~"+n+":~s~ \"You're making a serious mistake.\"" };
                case "Aggressive":  return new string[] { "~r~"+n+":~s~ \"Are you kidding me right now?!\"", "~r~"+n+":~s~ \"You've got some nerve. Get out of my face.\"", "~r~"+n+":~s~ \"Don't EVER pull that again.\"", "~r~"+n+":~s~ \"I can't believe you. You're done.\"" };
                case "Chaotic":     return new string[] { "~r~"+n+":~s~ \"WHAT?! You're ending it?! RIGHT NOW?!\"", "~r~"+n+":~s~ \"Oh wow okay fine then FINE!\"", "~r~"+n+":~s~ \"That is so— I can't even— UGH.\"", "~r~"+n+":~s~ \"You just blew up everything and walked away?!\"" };
                case "Unstable":    return new string[] { "~r~"+n+":~s~ \"No. No no no. You can't do this to me!\"", "~r~"+n+":~s~ \"Why?! What did I DO?!\"", "~r~"+n+":~s~ \"I can't handle this right now. I can't.\"", "~r~"+n+":~s~ \"You don't get to just END this!\"" };
                default:            return new string[] { "~r~"+n+":~s~ \"Are you serious?! You're cutting me off like this?\"", "~r~"+n+":~s~ \"Don't ever talk to me again. We're done.\"", "~r~"+n+":~s~ \"You have some nerve. Get out of my sight.\"", "~r~"+n+":~s~ \"I can't believe you. This is how you end it?\"" };
            }
        }

        private static string[] GetArrangementEndRefuseLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~r~"+n+":~s~ \"I... I don't want it to end. Can we keep it going?\"", "~r~"+n+":~s~ \"Please don't end this. I need the money.\"", "~r~"+n+":~s~ \"I know it's weird but... can we stay?\"", "~r~"+n+":~s~ \"I'd rather keep things as they are, if that's okay.\"" };
                case "Sweet":       return new string[] { "~r~"+n+":~s~ \"I appreciate you, but... I'm not ready to stop.\"", "~r~"+n+":~s~ \"Can we just... keep going a little longer?\"", "~r~"+n+":~s~ \"I kind of like what we have. Let's not end it.\"", "~r~"+n+":~s~ \"Please? Just a bit longer?\"" };
                case "Romantic":    return new string[] { "~r~"+n+":~s~ \"Don't end this. Not now. Not yet.\"", "~r~"+n+":~s~ \"What we have is special. Don't throw it away.\"", "~r~"+n+":~s~ \"I'm not ready to let go of this.\"", "~r~"+n+":~s~ \"We can work it out. Just don't end it.\"" };
                case "Needy":       return new string[] { "~r~"+n+":~s~ \"Please please please don't end this.\"", "~r~"+n+":~s~ \"I'll do whatever you want. Just don't leave.\"", "~r~"+n+":~s~ \"No! We can't end this! I need you!\"", "~r~"+n+":~s~ \"Don't do this to me. Not now.\"" };
                case "Flirty":      return new string[] { "~r~"+n+":~s~ \"Mmm, I don't think so. I'm staying.\"", "~r~"+n+":~s~ \"Why would I walk away from something this good?\"", "~r~"+n+":~s~ \"You're not getting rid of me that easy.\"", "~r~"+n+":~s~ \"Nope. I like this arrangement.\"" };
                case "Playful":     return new string[] { "~r~"+n+":~s~ \"Haha, no. We're keeping this going.\"", "~r~"+n+":~s~ \"Nice try. I'm not going anywhere.\"", "~r~"+n+":~s~ \"End it? Nah, we're just getting started.\"", "~r~"+n+":~s~ \"You can't kick me out. I'm too fun.\"" };
                case "Party Girl":  return new string[] { "~r~"+n+":~s~ \"No way! We still have a good thing going!\"", "~r~"+n+":~s~ \"I'm not done having fun. We're keeping this.\"", "~r~"+n+":~s~ \"Walk away? Pfft. Never.\"", "~r~"+n+":~s~ \"You're crazy if you think I'm leaving now.\"" };
                case "Sarcastic":   return new string[] { "~r~"+n+":~s~ \"Oh, you thought I'd just walk away? Adorable.\"", "~r~"+n+":~s~ \"Ha. No. Try again.\"", "~r~"+n+":~s~ \"Ending this? How naive of you.\"", "~r~"+n+":~s~ \"Sure. I'll get right on that. Not.\"" };
                case "Cold":        return new string[] { "~r~"+n+":~s~ \"No. Arrangement stands.\"", "~r~"+n+":~s~ \"I decide when this ends. Not you.\"", "~r~"+n+":~s~ \"We're not done yet.\"", "~r~"+n+":~s~ \"I'll let you know when I'm finished.\"" };
                case "Independent": return new string[] { "~r~"+n+":~s~ \"This works for me on my terms. I'm staying.\"", "~r~"+n+":~s~ \"I walk when I want to walk.\"", "~r~"+n+":~s~ \"You don't get to call that for me.\"", "~r~"+n+":~s~ \"I'll end it when I'm good and ready.\"" };
                case "Classy":      return new string[] { "~r~"+n+":~s~ \"I'm afraid that decision isn't yours to make.\"", "~r~"+n+":~s~ \"I don't take direction from you on matters like this.\"", "~r~"+n+":~s~ \"I'll decide when our arrangement concludes.\"", "~r~"+n+":~s~ \"How presumptuous. No.\"" };
                case "Gold Digger": return new string[] { "~r~"+n+":~s~ \"Walk away? The money's too good. I'm staying.\"", "~r~"+n+":~s~ \"No. We keep the arrangement. I decide when it ends.\"", "~r~"+n+":~s~ \"You don't call this off. I do.\"", "~r~"+n+":~s~ \"Ha. You think I'll just walk away from steady income?\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"That's not how this works. I stay until I say otherwise.\"", "~r~"+n+":~s~ \"You don't make that call. I do.\"", "~r~"+n+":~s~ \"We have an agreement. You don't just break it.\"", "~r~"+n+":~s~ \"Nice try. But no.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"I don't walk away from good money. Nice try.\"", "~r~"+n+":~s~ \"You think you can just end this? That's not how it works.\"", "~r~"+n+":~s~ \"I've been around long enough to know when to stay.\"", "~r~"+n+":~s~ \"Nah. I'm good right here.\"" };
                case "Mysterious":  return new string[] { "~r~"+n+":~s~ \"It ends when I decide it ends.\"", "~r~"+n+":~s~ \"You don't know what you're giving up.\"", "~r~"+n+":~s~ \"I'm not ready for this to be over.\"", "~r~"+n+":~s~ \"Some things aren't yours to close.\"" };
                case "Jealous":     return new string[] { "~r~"+n+":~s~ \"You're not ending this to be with someone else.\"", "~r~"+n+":~s~ \"I know what this is about. And the answer is no.\"", "~r~"+n+":~s~ \"Not a chance. We're not done.\"", "~r~"+n+":~s~ \"I'll end it when I'm satisfied. Not before.\"" };
                case "Dominant":    return new string[] { "~r~"+n+":~s~ \"You don't end this. I do. Get that straight.\"", "~r~"+n+":~s~ \"This arrangement continues on my schedule.\"", "~r~"+n+":~s~ \"Nobody walks out on me. Nobody.\"", "~r~"+n+":~s~ \"I said no. That's final.\"" };
                case "Aggressive":  return new string[] { "~r~"+n+":~s~ \"You're not ending anything. Back off.\"", "~r~"+n+":~s~ \"Try walking away and see what happens.\"", "~r~"+n+":~s~ \"I don't take orders from you.\"", "~r~"+n+":~s~ \"This arrangement stays. End of story.\"" };
                case "Chaotic":     return new string[] { "~r~"+n+":~s~ \"NOPE. We're keeping it! I like chaos!\"", "~r~"+n+":~s~ \"End it? No way! Where's the fun in that?!\"", "~r~"+n+":~s~ \"You can't end something you can't control!\"", "~r~"+n+":~s~ \"Ha! Good luck trying to walk away from me!\"" };
                case "Unstable":    return new string[] { "~r~"+n+":~s~ \"No! You don't get to decide that!\"", "~r~"+n+":~s~ \"Stop trying to leave! Don't leave!\"", "~r~"+n+":~s~ \"If you end this I don't know what I'll do!\"", "~r~"+n+":~s~ \"You CAN'T end this. We need this!\"" };
                default:            return new string[] { "~r~"+n+":~s~ \"Walk away? The money's too good. I'm staying.\"", "~r~"+n+":~s~ \"No. We keep the arrangement. I decide when it ends.\"", "~r~"+n+":~s~ \"You don't call this off. I do.\"", "~r~"+n+":~s~ \"Ha. You think I'll just walk away from steady income?\"" };
            }
        }

        private static string[] GetArrangementEndFriendlyLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~g~"+n+":~s~ \"Oh... yeah, okay. Maybe that's for the best.\"", "~g~"+n+":~s~ \"I think I knew this was coming. It's okay.\"", "~g~"+n+":~s~ \"...Thanks for being nice about it.\"", "~g~"+n+":~s~ \"Okay. No more business. We can still talk, right?\"" };
                case "Sweet":       return new string[] { "~g~"+n+":~s~ \"I'm glad we had this. No hard feelings.\"", "~g~"+n+":~s~ \"That's actually really mature of you. I appreciate it.\"", "~g~"+n+":~s~ \"You're a good person. Thank you for this.\"", "~g~"+n+":~s~ \"Okay. Friends then. I'd like that.\"" };
                case "Romantic":    return new string[] { "~g~"+n+":~s~ \"...Maybe this is how it was supposed to end.\"", "~g~"+n+":~s~ \"I'd rather have you as a friend than lose you entirely.\"", "~g~"+n+":~s~ \"Okay. No more business. But I'll still think about what we had.\"", "~g~"+n+":~s~ \"That actually means a lot. Thank you.\"" };
                case "Needy":       return new string[] { "~g~"+n+":~s~ \"You mean we can still hang out? Just... differently?\"", "~g~"+n+":~s~ \"Okay. Okay. As long as you don't disappear.\"", "~g~"+n+":~s~ \"I'll miss it, but I'll take friendship over nothing.\"", "~g~"+n+":~s~ \"You're not leaving me though? Just the arrangement?\"" };
                case "Flirty":      return new string[] { "~g~"+n+":~s~ \"Aww. No more business? I'll miss the fun.\"", "~g~"+n+":~s~ \"Fine, but I reserve the right to still flirt.\"", "~g~"+n+":~s~ \"Friends it is. Lucky you.\"", "~g~"+n+":~s~ \"Okay. I actually kind of respect that.\"" };
                case "Playful":     return new string[] { "~g~"+n+":~s~ \"Ha, okay fair. We can be just friends.\"", "~g~"+n+":~s~ \"No more business? Boooo. But fine.\"", "~g~"+n+":~s~ \"You know what? Fair enough. I actually like you.\"", "~g~"+n+":~s~ \"Deal. But I'm still gonna mess with you sometimes.\"" };
                case "Party Girl":  return new string[] { "~g~"+n+":~s~ \"Okay okay. Friends only. We can still party though, right?\"", "~g~"+n+":~s~ \"As long as we're still hanging out, I'm fine.\"", "~g~"+n+":~s~ \"Yeah, I think I needed this too. Let's just have fun.\"", "~g~"+n+":~s~ \"Alright. Just friends. But fun friends.\"" };
                case "Sarcastic":   return new string[] { "~g~"+n+":~s~ \"Well. That's almost touching. Fine.\"", "~g~"+n+":~s~ \"Friends. Sure. Didn't see that plot twist coming.\"", "~g~"+n+":~s~ \"Okay, I respect the honesty. Weirdly.\"", "~g~"+n+":~s~ \"Alright. No more business. You're lucky I like you.\"" };
                case "Cold":        return new string[] { "~g~"+n+":~s~ \"Fine. Arrangement's done.\"", "~g~"+n+":~s~ \"I can live with that.\"", "~g~"+n+":~s~ \"Okay. No strings.\"", "~g~"+n+":~s~ \"Noted. We're done.\"" };
                case "Independent": return new string[] { "~g~"+n+":~s~ \"Yeah, honestly this works better for me too.\"", "~g~"+n+":~s~ \"Fine by me. Keeps things clean.\"", "~g~"+n+":~s~ \"Good call. I prefer things uncomplicated.\"", "~g~"+n+":~s~ \"Works for me. No more business.\"" };
                case "Classy":      return new string[] { "~g~"+n+":~s~ \"Well handled. I appreciate the consideration.\"", "~g~"+n+":~s~ \"A graceful exit. I respect that.\"", "~g~"+n+":~s~ \"This is... actually a relief. Thank you.\"", "~g~"+n+":~s~ \"Very well. I think that's the right call.\"" };
                case "Gold Digger": return new string[] { "~g~"+n+":~s~ \"You know what? Fair enough. I actually like you.\"", "~g~"+n+":~s~ \"...I've been thinking the same thing. Let's just be friends.\"", "~g~"+n+":~s~ \"Okay. No more business. But I'll still be around.\"", "~g~"+n+":~s~ \"I respect that. Arrangement's done. We good?\"" };
                case "Manipulative":return new string[] { "~g~"+n+":~s~ \"Smart move. I was going to suggest the same thing.\"", "~g~"+n+":~s~ \"I can work with that. Friends is still useful.\"", "~g~"+n+":~s~ \"Sure. Let's call it done. On good terms.\"", "~g~"+n+":~s~ \"Okay. I'll accept that. For now.\"" };
                case "Street Smart":return new string[] { "~g~"+n+":~s~ \"Yeah, clean break. That's smart.\"", "~g~"+n+":~s~ \"I can respect that. No drama.\"", "~g~"+n+":~s~ \"Alright. We're square. That's fine.\"", "~g~"+n+":~s~ \"Good enough. Friends. I can do that.\"" };
                case "Mysterious":  return new string[] { "~g~"+n+":~s~ \"...Alright. This is a better ending than most.\"", "~g~"+n+":~s~ \"No more business. I can accept that.\"", "~g~"+n+":~s~ \"I'll be around. Just differently now.\"", "~g~"+n+":~s~ \"Some endings are beginnings. Sure.\"" };
                case "Jealous":     return new string[] { "~g~"+n+":~s~ \"Friends? As long as there's no one else involved.\"", "~g~"+n+":~s~ \"Okay. But I'll still be watching.\"", "~g~"+n+":~s~ \"Fine. Friends. Just... don't replace me.\"", "~g~"+n+":~s~ \"Alright. I can do friends. Probably.\"" };
                case "Dominant":    return new string[] { "~g~"+n+":~s~ \"Hmm. Fair enough. I'll allow it.\"", "~g~"+n+":~s~ \"I can agree to that. On my terms.\"", "~g~"+n+":~s~ \"Okay. No more business. I make the rules from here.\"", "~g~"+n+":~s~ \"Fine. Arrangement closed. My decision stands.\"" };
                case "Aggressive":  return new string[] { "~g~"+n+":~s~ \"...Fine. You want out? Then we're out.\"", "~g~"+n+":~s~ \"I respect the directness. Done.\"", "~g~"+n+":~s~ \"Alright. No hard feelings. Mostly.\"", "~g~"+n+":~s~ \"Okay. We're done with business. I can deal with that.\"" };
                case "Chaotic":     return new string[] { "~g~"+n+":~s~ \"OH! Friends! I love that! Yes! Deal!\"", "~g~"+n+":~s~ \"Business is boring anyway! Friends is WAY better!\"", "~g~"+n+":~s~ \"Wait for real?! That's actually amazing!\"", "~g~"+n+":~s~ \"YES. Done. Friendship activated. Let's go!\"" };
                case "Unstable":    return new string[] { "~g~"+n+":~s~ \"You're not leaving me? We're still friends?\"", "~g~"+n+":~s~ \"Okay. Okay. Friends is fine. You'll still be here?\"", "~g~"+n+":~s~ \"I thought you were going to disappear. Thank you.\"", "~g~"+n+":~s~ \"Just friends is okay. That's okay. Right?\"" };
                default:            return new string[] { "~g~"+n+":~s~ \"You know what? Fair enough. I actually like you.\"", "~g~"+n+":~s~ \"...I've been thinking the same thing. Let's just be friends.\"", "~g~"+n+":~s~ \"Okay. No more business. But I'll still be around.\"", "~g~"+n+":~s~ \"I respect that. Arrangement's done. We good?\"" };
            }
        }

        private static string[] GetReproposalAcceptLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~g~"+n+":~s~ \"I... okay. If you insist. But be gentle about it.\"", "~g~"+n+":~s~ \"I didn't expect that. Um. Okay, fine.\"", "~g~"+n+":~s~ \"I mean... I could use the money. Okay.\"", "~g~"+n+":~s~ \"That's unexpected. But... alright.\"" };
                case "Sweet":       return new string[] { "~g~"+n+":~s~ \"You really want to go back to that? Okay, sweetie.\"", "~g~"+n+":~s~ \"I wasn't expecting that, but okay. Sure.\"", "~g~"+n+":~s~ \"Alright. I can do that for you.\"", "~g~"+n+":~s~ \"If it means we stay close, then yeah.\"" };
                case "Romantic":    return new string[] { "~g~"+n+":~s~ \"Business again? I thought we were past that.\"", "~g~"+n+":~s~ \"...Fine. But this feels different now.\"", "~g~"+n+":~s~ \"Okay. Just don't let it change things between us.\"", "~g~"+n+":~s~ \"I'll try. But don't forget I'm more than that.\"" };
                case "Needy":       return new string[] { "~g~"+n+":~s~ \"Yes! Okay! Whatever keeps you around!\"", "~g~"+n+":~s~ \"If this means more time with you — deal.\"", "~g~"+n+":~s~ \"Okay okay. I'm in. Just don't leave after.\"", "~g~"+n+":~s~ \"Deal. But promise you'll still be here?\"" };
                case "Flirty":      return new string[] { "~g~"+n+":~s~ \"Oh, so now you want to pay? I can work with that.\"", "~g~"+n+":~s~ \"Back to business? Sure, if you're buying.\"", "~g~"+n+":~s~ \"Alright, I'm in. But I'm going to enjoy this.\"", "~g~"+n+":~s~ \"Strictly business? Sure. We'll see how long that lasts.\"" };
                case "Playful":     return new string[] { "~g~"+n+":~s~ \"Ohhh it's like that again? Sure, I'm game.\"", "~g~"+n+":~s~ \"Business round two? Let's go!\"", "~g~"+n+":~s~ \"Ha. Okay. I wasn't expecting that.\"", "~g~"+n+":~s~ \"You want to start over? Alright, deal.\"" };
                case "Party Girl":  return new string[] { "~g~"+n+":~s~ \"Sure! Why not? Life's too short.\"", "~g~"+n+":~s~ \"Business again? Fine, as long as we have fun.\"", "~g~"+n+":~s~ \"Okay yeah I'm down. Easy money.\"", "~g~"+n+":~s~ \"Deal! Now we're talking.\"" };
                case "Sarcastic":   return new string[] { "~g~"+n+":~s~ \"Oh look, we're back to this. How predictable.\"", "~g~"+n+":~s~ \"Fine. At least you're consistent.\"", "~g~"+n+":~s~ \"Sure. I can work with that.\"", "~g~"+n+":~s~ \"I didn't see that coming. Okay. Deal.\"" };
                case "Cold":        return new string[] { "~g~"+n+":~s~ \"Fine. We can try that again.\"", "~g~"+n+":~s~ \"If that's what you want. Okay.\"", "~g~"+n+":~s~ \"Alright. Strictly business.\"", "~g~"+n+":~s~ \"I can do that.\"" };
                case "Independent": return new string[] { "~g~"+n+":~s~ \"...Fine. I'll bite. But I set the terms.\"", "~g~"+n+":~s~ \"Okay. But we do this on my schedule.\"", "~g~"+n+":~s~ \"Sure. Business only. I like that.\"", "~g~"+n+":~s~ \"Alright. But I'm setting the prices.\"" };
                case "Classy":      return new string[] { "~g~"+n+":~s~ \"Well. I can entertain that proposal.\"", "~g~"+n+":~s~ \"If the terms are right, I can agree to that.\"", "~g~"+n+":~s~ \"I suppose we can revisit the arrangement.\"", "~g~"+n+":~s~ \"Very well. On my terms.\"" };
                case "Gold Digger": return new string[] { "~g~"+n+":~s~ \"Oh, you want to pay me again? Obviously yes.\"", "~g~"+n+":~s~ \"...Fine. You want to pay? I can work with that.\"", "~g~"+n+":~s~ \"You know what, sure. Strictly business though.\"", "~g~"+n+":~s~ \"I wasn't expecting that. Okay. Deal.\"" };
                case "Manipulative":return new string[] { "~g~"+n+":~s~ \"Interesting. You want to come back? I can make that work.\"", "~g~"+n+":~s~ \"Sure. On my terms, of course.\"", "~g~"+n+":~s~ \"I thought you'd come around. Okay.\"", "~g~"+n+":~s~ \"You need me more than you let on. Fine.\"" };
                case "Street Smart":return new string[] { "~g~"+n+":~s~ \"Back to business? I can respect that.\"", "~g~"+n+":~s~ \"Smart. Keep it professional. Sure.\"", "~g~"+n+":~s~ \"Alright. But I'm setting the terms this time.\"", "~g~"+n+":~s~ \"Okay. Deal. But don't waste my time.\"" };
                case "Mysterious":  return new string[] { "~g~"+n+":~s~ \"...I wasn't expecting that. Alright.\"", "~g~"+n+":~s~ \"You're full of surprises. Okay. Deal.\"", "~g~"+n+":~s~ \"I'll agree. For now.\"", "~g~"+n+":~s~ \"Fine. Business it is.\"" };
                case "Jealous":     return new string[] { "~g~"+n+":~s~ \"You're not doing this with anyone else, right?\"", "~g~"+n+":~s~ \"Fine. But this is exclusive. Got it?\"", "~g~"+n+":~s~ \"Okay. Deal. But I'm the only one.\"", "~g~"+n+":~s~ \"I'll agree — as long as it stays between us.\"" };
                case "Dominant":    return new string[] { "~g~"+n+":~s~ \"You want to come back? Then you follow my rules.\"", "~g~"+n+":~s~ \"Alright. But I'm setting the prices.\"", "~g~"+n+":~s~ \"Fine. We do this my way.\"", "~g~"+n+":~s~ \"Sure. But don't forget who's in charge.\"" };
                case "Aggressive":  return new string[] { "~g~"+n+":~s~ \"Fine. Business. But on my terms.\"", "~g~"+n+":~s~ \"Okay. But don't push me.\"", "~g~"+n+":~s~ \"Sure. We can go back to that.\"", "~g~"+n+":~s~ \"Alright. But get it straight from the start.\"" };
                case "Chaotic":     return new string[] { "~g~"+n+":~s~ \"YES! Back in business! Wooo!\"", "~g~"+n+":~s~ \"Wait really?! Okay! I'm so ready!\"", "~g~"+n+":~s~ \"Deal deal deal! Let's GO!\"", "~g~"+n+":~s~ \"Oh this just got interesting again!\"" };
                case "Unstable":    return new string[] { "~g~"+n+":~s~ \"You... you want me back? Okay. Okay yes!\"", "~g~"+n+":~s~ \"Really? You're not just saying that?\"", "~g~"+n+":~s~ \"Okay! Yes! I'm in! Don't change your mind!\"", "~g~"+n+":~s~ \"I'll do it. Just please don't leave after.\"" };
                default:            return new string[] { "~g~"+n+":~s~ \"...Fine. You want to pay? I can work with that.\"", "~g~"+n+":~s~ \"You know what, sure. Strictly business though.\"", "~g~"+n+":~s~ \"Alright. But I'm setting the prices.\"", "~g~"+n+":~s~ \"I wasn't expecting that. Okay. Deal.\"" };
            }
        }

        private static string[] GetReproposalDeclineLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~r~"+n+":~s~ \"I... I'm sorry. I just want to be friends.\"", "~r~"+n+":~s~ \"I don't think I can do that anymore.\"", "~r~"+n+":~s~ \"Please don't ask me that. It's different now.\"", "~r~"+n+":~s~ \"I like you as a friend. Let's keep it that way.\"" };
                case "Sweet":       return new string[] { "~r~"+n+":~s~ \"That's sweet, but no. Friends only.\"", "~r~"+n+":~s~ \"I care about you, but not like that anymore.\"", "~r~"+n+":~s~ \"I'd rather keep what we have. And what we have is friendship.\"", "~r~"+n+":~s~ \"No thanks. But I still like you.\"" };
                case "Romantic":    return new string[] { "~r~"+n+":~s~ \"I can't go back to that. It would ruin everything.\"", "~r~"+n+":~s~ \"What we have now is real. Let's not trade it for that.\"", "~r~"+n+":~s~ \"I said friends. And I meant it.\"", "~r~"+n+":~s~ \"No. Not like that. Not anymore.\"" };
                case "Needy":       return new string[] { "~r~"+n+":~s~ \"Please don't push me on this. I just want you to stay.\"", "~r~"+n+":~s~ \"I can't. Not anymore. Just be my friend?\"", "~r~"+n+":~s~ \"Don't make this weird. We're friends now.\"", "~r~"+n+":~s~ \"Please... just friends. Okay?\"" };
                case "Flirty":      return new string[] { "~r~"+n+":~s~ \"Tempting, but no. Friends only.\"", "~r~"+n+":~s~ \"Nice try. You had your chance.\"", "~r~"+n+":~s~ \"I still like you, just not like that.\"", "~r~"+n+":~s~ \"Nope! That ship sailed.\"" };
                case "Playful":     return new string[] { "~r~"+n+":~s~ \"Haha, no. We're friends now, dummy.\"", "~r~"+n+":~s~ \"Nice try! Not happening.\"", "~r~"+n+":~s~ \"I said friends! That's it!\"", "~r~"+n+":~s~ \"Not a chance. But I still like you as a friend.\"" };
                case "Party Girl":  return new string[] { "~r~"+n+":~s~ \"Nah. We're just friends. That's more fun anyway.\"", "~r~"+n+":~s~ \"Still no. But hey, we can still hang.\"", "~r~"+n+":~s~ \"Not happening. But friends is cool.\"", "~r~"+n+":~s~ \"Ha, no. Come on, let's just have fun.\"" };
                case "Sarcastic":   return new string[] { "~r~"+n+":~s~ \"Oh, so now you want to pay? No.\"", "~r~"+n+":~s~ \"Still no. Don't push it.\"", "~r~"+n+":~s~ \"Wow. You tried again. Brave. Still no.\"", "~r~"+n+":~s~ \"I like you. But not like that.\"" };
                case "Cold":        return new string[] { "~r~"+n+":~s~ \"No.\"", "~r~"+n+":~s~ \"We're friends. That's it.\"", "~r~"+n+":~s~ \"Not interested.\"", "~r~"+n+":~s~ \"Don't ask again.\"" };
                case "Independent": return new string[] { "~r~"+n+":~s~ \"I said friends. That's it.\"", "~r~"+n+":~s~ \"I don't go backwards. Friendship only.\"", "~r~"+n+":~s~ \"Not happening. We're just friends.\"", "~r~"+n+":~s~ \"My answer doesn't change.\"" };
                case "Classy":      return new string[] { "~r~"+n+":~s~ \"I've already given you my answer.\"", "~r~"+n+":~s~ \"That proposal remains declined.\"", "~r~"+n+":~s~ \"Persisting won't change the outcome.\"", "~r~"+n+":~s~ \"No. And please don't ask again.\"" };
                case "Gold Digger": return new string[] { "~r~"+n+":~s~ \"I said friends. That's it.\"", "~r~"+n+":~s~ \"Still no. Don't push it.\"", "~r~"+n+":~s~ \"Not happening. We're just friends.\"", "~r~"+n+":~s~ \"I like you. But not like that.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"Hmm. Still no. You haven't earned it back.\"", "~r~"+n+":~s~ \"Maybe someday. Not today.\"", "~r~"+n+":~s~ \"No. But keep trying. It's amusing.\"", "~r~"+n+":~s~ \"I'll let you know when my answer changes.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"Nah. I see what you're doing.\"", "~r~"+n+":~s~ \"I'm smart enough to know when to say no.\"", "~r~"+n+":~s~ \"Not biting. We're friends.\"", "~r~"+n+":~s~ \"I hear you. And the answer's still no.\"" };
                case "Mysterious":  return new string[] { "~r~"+n+":~s~ \"No. That chapter's closed.\"", "~r~"+n+":~s~ \"Some doors don't reopen.\"", "~r~"+n+":~s~ \"I've moved past that.\"", "~r~"+n+":~s~ \"Ask me something else.\"" };
                case "Jealous":     return new string[] { "~r~"+n+":~s~ \"Are you doing this with anyone else?\"", "~r~"+n+":~s~ \"If this is exclusive, maybe. But otherwise — no.\"", "~r~"+n+":~s~ \"Not until I know it's just me.\"", "~r~"+n+":~s~ \"I'm not sharing you. So still no.\"" };
                case "Dominant":    return new string[] { "~r~"+n+":~s~ \"That's not your call to make.\"", "~r~"+n+":~s~ \"I decide what happens next. And the answer is no.\"", "~r~"+n+":~s~ \"My decision hasn't changed.\"", "~r~"+n+":~s~ \"No means no.\"" };
                case "Aggressive":  return new string[] { "~r~"+n+":~s~ \"I said no. Don't make me say it again.\"", "~r~"+n+":~s~ \"Back off. We're just friends.\"", "~r~"+n+":~s~ \"Still no. Don't test me.\"", "~r~"+n+":~s~ \"Push it again and we're not even friends.\"" };
                case "Chaotic":     return new string[] { "~r~"+n+":~s~ \"NO! Friends! FRIENDS! How many times!\"", "~r~"+n+":~s~ \"You had ONE job — be my friend! That's it!\"", "~r~"+n+":~s~ \"STILL NO! Wow! The audacity!\"", "~r~"+n+":~s~ \"Nope nope nope! Friends zone! Locked!\"" };
                case "Unstable":    return new string[] { "~r~"+n+":~s~ \"Please don't ask me that. It makes everything complicated.\"", "~r~"+n+":~s~ \"No. Just... no. Please just be my friend.\"", "~r~"+n+":~s~ \"Why do you keep pushing?! Just be here for me!\"", "~r~"+n+":~s~ \"I can't handle that right now. Just friends. Please.\"" };
                default:            return new string[] { "~r~"+n+":~s~ \"I said friends. That's it.\"", "~r~"+n+":~s~ \"Still no. Don't push it.\"", "~r~"+n+":~s~ \"Not happening. We're just friends.\"", "~r~"+n+":~s~ \"I like you. But not like that.\"" };
            }
        }

        private static string[] GetPropositionAcceptLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~g~"+n+":~s~ \"I... okay. If you're serious. I could use the money.\"", "~g~"+n+":~s~ \"That's unexpected. But fine, I'll try.\"", "~g~"+n+":~s~ \"I don't usually do this. But okay.\"", "~g~"+n+":~s~ \"Um. Sure. Just... be nice about it.\"" };
                case "Sweet":       return new string[] { "~g~"+n+":~s~ \"That's sweet of you to offer. Okay, sure.\"", "~g~"+n+":~s~ \"You caught me in a good mood. Why not?\"", "~g~"+n+":~s~ \"I'll try it once. Don't make it weird.\"", "~g~"+n+":~s~ \"Okay. But let's keep it kind.\"" };
                case "Romantic":    return new string[] { "~g~"+n+":~s~ \"This isn't how I imagined things, but... okay.\"", "~g~"+n+":~s~ \"I'd rather it was more than business. But sure.\"", "~g~"+n+":~s~ \"If this is how we start... fine.\"", "~g~"+n+":~s~ \"You caught me off guard. Okay. Let's try.\"" };
                case "Needy":       return new string[] { "~g~"+n+":~s~ \"You want to pay me? Yes! Okay!\"", "~g~"+n+":~s~ \"If it means spending more time with you — deal.\"", "~g~"+n+":~s~ \"Okay yes! Just don't disappear after.\"", "~g~"+n+":~s~ \"Deal. But you have to stay. Promise?\"" };
                case "Flirty":      return new string[] { "~g~"+n+":~s~ \"Oh, so it's like that? I like where this is going.\"", "~g~"+n+":~s~ \"Money AND fun? Sold.\"", "~g~"+n+":~s~ \"Ha. I was wondering when you'd ask. Sure.\"", "~g~"+n+":~s~ \"Let's make it work. I'm in.\"" };
                case "Playful":     return new string[] { "~g~"+n+":~s~ \"Ooh, this is new! Sure, let's try it!\"", "~g~"+n+":~s~ \"Ha! You actually asked! Okay, fine!\"", "~g~"+n+":~s~ \"You caught me in a weird mood. Sure.\"", "~g~"+n+":~s~ \"I'll try it once. Could be fun!\"" };
                case "Party Girl":  return new string[] { "~g~"+n+":~s~ \"Easy money? Sure, I'm in!\"", "~g~"+n+":~s~ \"Yeah, why not? Life's short.\"", "~g~"+n+":~s~ \"Deal. Now we're talking!\"", "~g~"+n+":~s~ \"Ha! Didn't take long. Okay, yeah.\"" };
                case "Sarcastic":   return new string[] { "~g~"+n+":~s~ \"Ha. I was wondering when you'd bring that up. Okay.\"", "~g~"+n+":~s~ \"Took you long enough. Fine.\"", "~g~"+n+":~s~ \"Sure. Predictable, but sure.\"", "~g~"+n+":~s~ \"Let me pretend to think about it. ...Okay.\"" };
                case "Cold":        return new string[] { "~g~"+n+":~s~ \"Fine. Strictly business.\"", "~g~"+n+":~s~ \"Cash upfront. Deal.\"", "~g~"+n+":~s~ \"Okay. Don't make it personal.\"", "~g~"+n+":~s~ \"I can work with that.\"" };
                case "Independent": return new string[] { "~g~"+n+":~s~ \"Fine. On my terms.\"", "~g~"+n+":~s~ \"I'll try it once. Don't make it weird.\"", "~g~"+n+":~s~ \"Okay, but I set the rules.\"", "~g~"+n+":~s~ \"That's... unexpected. But fine, I'll bite.\"" };
                case "Classy":      return new string[] { "~g~"+n+":~s~ \"If the arrangement is appropriate, I can agree.\"", "~g~"+n+":~s~ \"I'll consider your proposal. Fine.\"", "~g~"+n+":~s~ \"Very well. But we do this with discretion.\"", "~g~"+n+":~s~ \"I can entertain that. Conditionally.\"" };
                case "Gold Digger": return new string[] { "~g~"+n+":~s~ \"Oh, I like where this is going. Sure, let's talk rates.\"", "~g~"+n+":~s~ \"Money first, fun second. I'm in.\"", "~g~"+n+":~s~ \"Cash talks. I'm listening. Let's make it work.\"", "~g~"+n+":~s~ \"Obviously yes. Let's do this.\"" };
                case "Manipulative":return new string[] { "~g~"+n+":~s~ \"I thought you'd come around eventually. Fine.\"", "~g~"+n+":~s~ \"I can work with this. Sure.\"", "~g~"+n+":~s~ \"This works for me. I'm in.\"", "~g~"+n+":~s~ \"Interesting offer. I'll accept.\"" };
                case "Street Smart":return new string[] { "~g~"+n+":~s~ \"Yeah, I can do that. Clean and simple.\"", "~g~"+n+":~s~ \"Business only. I respect that. Deal.\"", "~g~"+n+":~s~ \"Alright. But keep it professional.\"", "~g~"+n+":~s~ \"Sure. As long as we're clear on the terms.\"" };
                case "Mysterious":  return new string[] { "~g~"+n+":~s~ \"...Interesting offer. Fine.\"", "~g~"+n+":~s~ \"I'll accept. For reasons of my own.\"", "~g~"+n+":~s~ \"You're not what I expected. Okay.\"", "~g~"+n+":~s~ \"Alright. But don't read into it.\"" };
                case "Jealous":     return new string[] { "~g~"+n+":~s~ \"Fine. But I'm the only one you're doing this with.\"", "~g~"+n+":~s~ \"Okay — but this stays exclusive.\"", "~g~"+n+":~s~ \"I'll agree. As long as there's no one else.\"", "~g~"+n+":~s~ \"Sure. But if I find out you asked someone else, we're done.\"" };
                case "Dominant":    return new string[] { "~g~"+n+":~s~ \"Fine. But we do this my way.\"", "~g~"+n+":~s~ \"I'm in. But I make the rules.\"", "~g~"+n+":~s~ \"Alright. You'll follow my lead.\"", "~g~"+n+":~s~ \"Deal. Don't forget who's in charge here.\"" };
                case "Aggressive":  return new string[] { "~g~"+n+":~s~ \"Fine. But we do this straight. No games.\"", "~g~"+n+":~s~ \"Okay. But don't waste my time.\"", "~g~"+n+":~s~ \"Sure. Keep it clean and we're good.\"", "~g~"+n+":~s~ \"Alright. But I'm setting the terms.\"" };
                case "Chaotic":     return new string[] { "~g~"+n+":~s~ \"OHHH! YES! This is exciting! Okay!\"", "~g~"+n+":~s~ \"Wait really?! That's amazing! Deal!\"", "~g~"+n+":~s~ \"Ha! Sure! Let's see where THIS goes!\"", "~g~"+n+":~s~ \"YES! Done! Let's make it weird!\"" };
                case "Unstable":    return new string[] { "~g~"+n+":~s~ \"You... want me? Really? Okay. Yes!\"", "~g~"+n+":~s~ \"I'll do it. Just please don't leave after.\"", "~g~"+n+":~s~ \"Okay yes! But you have to promise to stay!\"", "~g~"+n+":~s~ \"Really? Okay. Okay! Don't change your mind!\"" };
                default:            return new string[] { "~g~"+n+":~s~ \"Hmm. Honestly? I'm interested. Let's try it.\"", "~g~"+n+":~s~ \"That's... not a bad idea. Sure.\"", "~g~"+n+":~s~ \"You're serious? ...Okay. I could use the extra.\"", "~g~"+n+":~s~ \"I'll think about it. Actually, you know what — yeah.\"" };
            }
        }

        private static string[] GetPropositionDeclineLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~r~"+n+":~s~ \"Oh... no. I don't think I can do that.\"", "~r~"+n+":~s~ \"I'm sorry. That's just not for me.\"", "~r~"+n+":~s~ \"Please don't ask me that.\"", "~r~"+n+":~s~ \"I'm flattered but... no thank you.\"" };
                case "Sweet":       return new string[] { "~r~"+n+":~s~ \"That's kind of you to ask, but no.\"", "~r~"+n+":~s~ \"I appreciate it, but that's not something I do.\"", "~r~"+n+":~s~ \"No, but thanks for being nice about it.\"", "~r~"+n+":~s~ \"I'll pass. But we can still be friends.\"" };
                case "Romantic":    return new string[] { "~r~"+n+":~s~ \"I want something real. Not this.\"", "~r~"+n+":~s~ \"I'm not a transaction. Sorry.\"", "~r~"+n+":~s~ \"That's not the kind of relationship I want.\"", "~r~"+n+":~s~ \"No. I hope you understand.\"" };
                case "Needy":       return new string[] { "~r~"+n+":~s~ \"Please don't do this. I just want you to like me.\"", "~r~"+n+":~s~ \"Can't we just... be close without that?\"", "~r~"+n+":~s~ \"I don't want money to change things.\"", "~r~"+n+":~s~ \"No... I'm scared that'll ruin everything.\"" };
                case "Flirty":      return new string[] { "~r~"+n+":~s~ \"Ha! Nice try. That's not my thing.\"", "~r~"+n+":~s~ \"I flirt for free. I don't charge for it.\"", "~r~"+n+":~s~ \"Tempting offer. But no.\"", "~r~"+n+":~s~ \"I'd rather keep the fun without the contract.\"" };
                case "Playful":     return new string[] { "~r~"+n+":~s~ \"Ha! Really? No. But points for trying.\"", "~r~"+n+":~s~ \"Not my scene. But funny you asked.\"", "~r~"+n+":~s~ \"Nope! But I like the confidence.\"", "~r~"+n+":~s~ \"That's a no from me. Moving on!\"" };
                case "Party Girl":  return new string[] { "~r~"+n+":~s~ \"Ha, nah. That's not really my vibe.\"", "~r~"+n+":~s~ \"I have fun, but not like that. No thanks.\"", "~r~"+n+":~s~ \"Not for me. But I'll still party with you.\"", "~r~"+n+":~s~ \"Nope. That's a line I don't cross.\"" };
                case "Sarcastic":   return new string[] { "~r~"+n+":~s~ \"Oh, wow. No. Hard no. Thanks for the laugh though.\"", "~r~"+n+":~s~ \"I'm going to pretend you didn't say that.\"", "~r~"+n+":~s~ \"Wow. Brave. And absolutely not.\"", "~r~"+n+":~s~ \"Ha! I appreciate the offer, but no.\"" };
                case "Cold":        return new string[] { "~r~"+n+":~s~ \"No.\"", "~r~"+n+":~s~ \"That's not something I do.\"", "~r~"+n+":~s~ \"Not interested.\"", "~r~"+n+":~s~ \"No thanks. That's not me.\"" };
                case "Independent": return new string[] { "~r~"+n+":~s~ \"I don't do that. My call, not yours.\"", "~r~"+n+":~s~ \"That's not the kind of deal I make.\"", "~r~"+n+":~s~ \"I'm good on my own. No.\"", "~r~"+n+":~s~ \"That's a no. I like keeping things clean.\"" };
                case "Classy":      return new string[] { "~r~"+n+":~s~ \"I find that offer rather insulting, honestly.\"", "~r~"+n+":~s~ \"That's not how I conduct myself.\"", "~r~"+n+":~s~ \"No. And I'd prefer not to revisit this.\"", "~r~"+n+":~s~ \"I'm going to politely decline.\"" };
                case "Gold Digger": return new string[] { "~r~"+n+":~s~ \"Hmm. The rate would have to be very good.\"", "~r~"+n+":~s~ \"I'm flattered, kind of. But that's not for me.\"", "~r~"+n+":~s~ \"Not really my scene. Sorry.\"", "~r~"+n+":~s~ \"Ha! I appreciate the offer, but no.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"Interesting timing. But no.\"", "~r~"+n+":~s~ \"I'll decide if that changes. For now — no.\"", "~r~"+n+":~s~ \"You'll have to do better than that.\"", "~r~"+n+":~s~ \"That's not going to work on me. No.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"I know what that leads to. No thanks.\"", "~r~"+n+":~s~ \"I don't mix that stuff up. No.\"", "~r~"+n+":~s~ \"Not biting. That's not how I operate.\"", "~r~"+n+":~s~ \"Smart enough to say no. So no.\"" };
                case "Mysterious":  return new string[] { "~r~"+n+":~s~ \"That's not who I am.\"", "~r~"+n+":~s~ \"Some things aren't for sale.\"", "~r~"+n+":~s~ \"No. And I won't explain why.\"", "~r~"+n+":~s~ \"Hmm. Not really my scene. Sorry.\"" };
                case "Jealous":     return new string[] { "~r~"+n+":~s~ \"Are you doing this with other people? Because if so — no.\"", "~r~"+n+":~s~ \"I don't like this. No.\"", "~r~"+n+":~s~ \"Not a chance. I don't share myself like that.\"", "~r~"+n+":~s~ \"No. It makes me think about who else you're asking.\"" };
                case "Dominant":    return new string[] { "~r~"+n+":~s~ \"You don't get to propose that to me.\"", "~r~"+n+":~s~ \"No. That's not how things work between us.\"", "~r~"+n+":~s~ \"I make the offers here. And I'm not offering that.\"", "~r~"+n+":~s~ \"That's a firm no.\"" };
                case "Aggressive":  return new string[] { "~r~"+n+":~s~ \"Are you kidding me? Get out of my face.\"", "~r~"+n+":~s~ \"Don't insult me like that.\"", "~r~"+n+":~s~ \"Wow. No. Absolutely not.\"", "~r~"+n+":~s~ \"You've got some nerve. The answer is no.\"" };
                case "Chaotic":     return new string[] { "~r~"+n+":~s~ \"HA! NO! That's insane! I love it! But still NO!\"", "~r~"+n+":~s~ \"Wooow okay that came out of nowhere. No!\"", "~r~"+n+":~s~ \"Nope nope nope! Too chaotic even for me!\"", "~r~"+n+":~s~ \"That is WILD. Absolutely not. Wow.\"" };
                case "Unstable":    return new string[] { "~r~"+n+":~s~ \"No! Why would you even ask me that?!\"", "~r~"+n+":~s~ \"That makes me feel weird. Stop.\"", "~r~"+n+":~s~ \"I can't. I just can't. Please don't push.\"", "~r~"+n+":~s~ \"No no no. Just... no.\"" };
                default:            return new string[] { "~r~"+n+":~s~ \"That's not something I do.\"", "~r~"+n+":~s~ \"No thanks. That's not me.\"", "~r~"+n+":~s~ \"I'm going to pretend you didn't say that.\"", "~r~"+n+":~s~ \"I don't think so. Let's just leave it.\"" };
            }
        }

        private static string[] GetGfHardBreakupLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Romantic":  return new string[] { "~r~"+n+":~s~ \"I loved what we had. But I can't do this anymore. We're done.\"", "~r~"+n+":~s~ \"I gave you my heart and you let it fall apart. This is over.\"", "~r~"+n+":~s~ \"I still care about you. That's exactly why I have to end this.\"", "~r~"+n+":~s~ \"We could have been something beautiful. But this? This isn't it. Goodbye.\"" };
                case "Needy":     return new string[] { "~r~"+n+":~s~ \"I gave you EVERYTHING. And you still couldn't... I'm done.\"", "~r~"+n+":~s~ \"Don't call me. Don't look for me. We are finished.\"", "~r~"+n+":~s~ \"I can't keep waiting for you to care. It's over.\"", "~r~"+n+":~s~ \"I needed you and you weren't there. Not once. Done.\"" };
                case "Cold":      return new string[] { "~r~"+n+":~s~ \"This isn't working. It's over. Don't make it complicated.\"", "~r~"+n+":~s~ \"We're done. Simple as that.\"", "~r~"+n+":~s~ \"I don't do second chances. Walk away.\"", "~r~"+n+":~s~ \"I made my decision. It's final. Leave.\"" };
                case "Playful":   return new string[] { "~r~"+n+":~s~ \"I tried to make this fun. It stopped being fun. Bye.\"", "~r~"+n+":~s~ \"I was rooting for us. But no. We're done.\"", "~r~"+n+":~s~ \"You ruined a good thing. I'm out.\"", "~r~"+n+":~s~ \"I keep laughing things off. Not this time. We're through.\"" };
                case "Shy":       return new string[] { "~r~"+n+":~s~ \"I... I can't do this anymore. I'm sorry. Please don't follow me.\"", "~r~"+n+":~s~ \"This is really hard to say. But we have to stop.\"", "~r~"+n+":~s~ \"I kept hoping it would get better. It didn't. I'm done.\"", "~r~"+n+":~s~ \"I don't want to hurt you but... I have to go. For good.\"" };
                case "Confident": return new string[] { "~r~"+n+":~s~ \"I deserve better than this. We're over.\"", "~r~"+n+":~s~ \"I'm walking away. Don't even try to stop me.\"", "~r~"+n+":~s~ \"I made up my mind. This relationship is done.\"", "~r~"+n+":~s~ \"I know my worth. And this isn't it. Goodbye.\"" };
                case "Cheerful":  return new string[] { "~r~"+n+":~s~ \"I tried SO hard to stay positive. I can't anymore. This is over.\"", "~r~"+n+":~s~ \"I wanted this to work so badly. It just... didn't. Bye.\"", "~r~"+n+":~s~ \"You know what? I'm done smiling through this. We're through.\"", "~r~"+n+":~s~ \"I kept telling myself it was fine. It's not. Done.\"" };
                case "Sarcastic": return new string[] { "~r~"+n+":~s~ \"Wow. Shocking development. Yeah, we're done.\"", "~r~"+n+":~s~ \"I'd say I'm surprised this crashed, but here we are. Bye.\"", "~r~"+n+":~s~ \"Great relationship. Really. Absolutely over. But yeah — over.\"", "~r~"+n+":~s~ \"I would say I didn't see this coming, but I'd be lying. Done.\"" };
                case "Maternal":  return new string[] { "~r~"+n+":~s~ \"I was trying so hard for both of us. I just can't anymore. I'm sorry.\"", "~r~"+n+":~s~ \"I hope you find what you're looking for. But it isn't me. Goodbye.\"", "~r~"+n+":~s~ \"I care about you. That's exactly why I have to go.\"", "~r~"+n+":~s~ \"I gave this everything I had. I have nothing left to give. Goodbye.\"" };
                case "Fierce":    return new string[] { "~r~"+n+":~s~ \"You blew it. We are absolutely done. Get out of my life.\"", "~r~"+n+":~s~ \"Don't test me. I said we're done. That's final.\"", "~r~"+n+":~s~ \"I gave you a chance. You wasted it. Goodbye.\"", "~r~"+n+":~s~ \"I don't repeat myself. We're done. Don't come back.\"" };
                case "Spiritual": return new string[] { "~r~"+n+":~s~ \"The universe is telling me this is wrong for both of us. I'm leaving.\"", "~r~"+n+":~s~ \"I prayed this would work. It's not meant to be. I have to go.\"", "~r~"+n+":~s~ \"Some things end so better things can begin. Goodbye.\"", "~r~"+n+":~s~ \"I feel it in my soul. This chapter is over. Goodbye.\"" };
                case "Anxious":   return new string[] { "~r~"+n+":~s~ \"I've been so stressed about us. I can't keep doing this. It's over.\"", "~r~"+n+":~s~ \"Every time we talk I'm bracing for something to go wrong. I need to stop.\"", "~r~"+n+":~s~ \"I'm sorry. I just can't handle this anymore. We're done.\"", "~r~"+n+":~s~ \"My anxiety can't take this anymore. I have to end it. I'm sorry.\"" };
                default:          return new string[] { "~r~"+n+":~s~ \"This can't work between us anymore. I'm sorry.\"", "~r~"+n+":~s~ \"I've been thinking about this. We're done.\"", "~r~"+n+":~s~ \"I can't keep doing this. It's over.\"", "~r~"+n+":~s~ \"Please don't make this harder than it already is. Goodbye.\"", "~r~"+n+":~s~ \"I needed more from this. I'm not getting it. We're through.\"", "~r~"+n+":~s~ \"This is the last conversation we're having like this. It's over.\"", "~r~"+n+":~s~ \"I think we both knew this was coming. Done.\"", "~r~"+n+":~s~ \"I'm not angry. Just done. Goodbye.\"" };
            }
        }

        private static string[] GetGfSoftBreakupLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Romantic":  return new string[] { "~r~"+n+":~s~ \"I care about you... but I think we're better as friends.\"", "~r~"+n+":~s~ \"Maybe we moved too fast. I think we should take a step back.\"", "~r~"+n+":~s~ \"I love being around you. Just... as friends, okay?\"", "~r~"+n+":~s~ \"I don't want to lose you. But I think friends is where we belong.\"" };
                case "Needy":     return new string[] { "~r~"+n+":~s~ \"I need something more stable. Can we just... be friends?\"", "~r~"+n+":~s~ \"I can't keep feeling this way. Let's just be friends, okay?\"", "~r~"+n+":~s~ \"Friends is safer. I need safe right now.\"", "~r~"+n+":~s~ \"I'm not ready to let you go completely. So. Friends?\"" };
                case "Cold":      return new string[] { "~r~"+n+":~s~ \"This isn't working. Let's just be friends.\"", "~r~"+n+":~s~ \"The friendship made more sense. Let's go back to that.\"", "~r~"+n+":~s~ \"I'd rather have you as a friend than not at all. Friends.\"", "~r~"+n+":~s~ \"Friends. That's my offer. Take it or leave it.\"" };
                case "Playful":   return new string[] { "~r~"+n+":~s~ \"No hard feelings, okay? I think we're better as friends.\"", "~r~"+n+":~s~ \"Let's just stay friends. Way less complicated.\"", "~r~"+n+":~s~ \"Can we do the friends thing? That part was actually good.\"", "~r~"+n+":~s~ \"We vibe better as friends. Let's be honest about that.\"" };
                case "Shy":       return new string[] { "~r~"+n+":~s~ \"I... I think maybe we should just be friends. I'm sorry.\"", "~r~"+n+":~s~ \"This is hard to say, but... friends is better for us.\"", "~r~"+n+":~s~ \"Please don't be upset. I just... friends suits us better.\"", "~r~"+n+":~s~ \"I like you a lot. But like... as a friend. Sorry.\"" };
                case "Confident": return new string[] { "~r~"+n+":~s~ \"I've decided. We work better as friends.\"", "~r~"+n+":~s~ \"I think we both know this makes more sense as a friendship.\"", "~r~"+n+":~s~ \"Friends. That's where I want us to land. Cool?\"", "~r~"+n+":~s~ \"I always know what I want. Right now I want us as friends.\"" };
                case "Cheerful":  return new string[] { "~r~"+n+":~s~ \"Hey, this doesn't have to be a bad thing! Let's just be friends.\"", "~r~"+n+":~s~ \"I like you! Just... as a friend. That okay?\"", "~r~"+n+":~s~ \"Friends can still be awesome. Let's do that instead.\"", "~r~"+n+":~s~ \"I'm trying to spin this positively. Friends! See? Positive.\"" };
                case "Sarcastic": return new string[] { "~r~"+n+":~s~ \"Shocking development, I know. Let's just be friends.\"", "~r~"+n+":~s~ \"Friends. Revolutionary concept, I know. But here we are.\"", "~r~"+n+":~s~ \"I'm sure you're devastated. We're just friends now. Groundbreaking.\"", "~r~"+n+":~s~ \"Friendzoned. Classic story. You'll be fine.\"" };
                case "Maternal":  return new string[] { "~r~"+n+":~s~ \"I still care about you. But friends is better for both of us.\"", "~r~"+n+":~s~ \"Let's not ruin this. Can we just be friends?\"", "~r~"+n+":~s~ \"I'll always want good things for you. As a friend.\"", "~r~"+n+":~s~ \"I think we're better together as friends than as partners.\"" };
                case "Fierce":    return new string[] { "~r~"+n+":~s~ \"I'm not going to pretend this is working. Friends. That's it.\"", "~r~"+n+":~s~ \"Let's be real — this stopped being a real relationship. Friends.\"", "~r~"+n+":~s~ \"I'm drawing the line here. Friends only. Understood?\"", "~r~"+n+":~s~ \"I decide what I am to people. Right now I'm your friend. Only.\"" };
                case "Spiritual": return new string[] { "~r~"+n+":~s~ \"I feel like we're meant to be in each other's lives. Just not this way.\"", "~r~"+n+":~s~ \"The connection is real. But the relationship isn't. Friends.\"", "~r~"+n+":~s~ \"I think our paths cross better as friends. I truly believe that.\"", "~r~"+n+":~s~ \"Sometimes friendship is the deeper bond. Let's honour that.\"" };
                case "Anxious":   return new string[] { "~r~"+n+":~s~ \"I'm always anxious around you lately. Friends would be... easier.\"", "~r~"+n+":~s~ \"Can we dial this back? Being friends sounds a lot less scary.\"", "~r~"+n+":~s~ \"I like you, but this level of everything is too much. Just friends?\"", "~r~"+n+":~s~ \"Friends feels safe. I really need safe right now. Okay?\"" };
                default:          return new string[] { "~r~"+n+":~s~ \"I think we're better off as friends.\"", "~r~"+n+":~s~ \"I like you, but I think we should just stay friends.\"", "~r~"+n+":~s~ \"Can we just... be friends? I think that's better.\"", "~r~"+n+":~s~ \"Look, I care about you. But let's just be friends.\"", "~r~"+n+":~s~ \"I need us to go back to being friends. I'm sorry.\"", "~r~"+n+":~s~ \"It's not that I don't like you. Friends is just where I'm at.\"", "~r~"+n+":~s~ \"I'd rather keep you as a friend than lose you completely.\"", "~r~"+n+":~s~ \"Let's pump the brakes. Friends feels right.\"" };
            }
        }

        private static string GetEscalateAcceptLine(string name, string personality, int lineIdx)
        {
            string n = name;
            if (lineIdx == 1) // "Mind if I... come closer?.."
            switch (personality)
            {
                case "Dominant":    return "~g~"+n+":~s~ \"You're asking? Good. Come here.\"";
                case "Shy":         return "~g~"+n+":~s~ \"...I don't mind.\"";
                case "Flirty":      return "~g~"+n+":~s~ \"I was wondering when you'd close the gap.\"";
                case "Sweet":       return "~g~"+n+":~s~ \"Of course. Come here.\"";
                case "Romantic":    return "~g~"+n+":~s~ \"I was hoping you would.\"";
                case "Cold":        return "~g~"+n+":~s~ \"...Sure. Just don't hover.\"";
                case "Playful":     return "~g~"+n+":~s~ \"Took you long enough.\"";
                case "Gold Digger": return "~g~"+n+":~s~ \"Mmm. Make it interesting.\"";
                case "Party Girl":  return "~g~"+n+":~s~ \"Yeah, get in here.\"";
                case "Needy":       return "~g~"+n+":~s~ \"Please. I want you close.\"";
                case "Aggressive":  return "~g~"+n+":~s~ \"Stop asking and do it.\"";
                case "Sarcastic":   return "~g~"+n+":~s~ \"Oh, look who finally made a move.\"";
                case "Mysterious":  return "~g~"+n+":~s~ \"...Yeah.\"";
                case "Chaotic":     return "~g~"+n+":~s~ \"Yes! Get over here!\"";
                case "Manipulative":return "~g~"+n+":~s~ \"I'll let you. Don't waste it.\"";
                case "Classy":      return "~g~"+n+":~s~ \"You may.\"";
                case "Jealous":     return "~g~"+n+":~s~ \"Only you. Nobody else.\"";
                case "Independent": return "~g~"+n+":~s~ \"Sure. Just don't crowd me.\"";
                case "Street Smart":return "~g~"+n+":~s~ \"Go ahead. But read the room.\"";
                case "Unstable":    return "~g~"+n+":~s~ \"YES. Come here right now.\"";
                default:            return "~g~"+n+":~s~ \"...Yeah. Come here.\"";
            }
            if (lineIdx == 2) // "Wanna fuck?"
            switch (personality)
            {
                case "Dominant":    return "~g~"+n+":~s~ \"Straight to it. I respect that. Come on.\"";
                case "Shy":         return "~g~"+n+":~s~ \"That's... really blunt. But... okay.\"";
                case "Flirty":      return "~g~"+n+":~s~ \"God, finally someone who just says it. Yes.\"";
                case "Sweet":       return "~g~"+n+":~s~ \"You could've asked nicer but... yes.\"";
                case "Romantic":    return "~g~"+n+":~s~ \"Not how I imagined it, but... yes.\"";
                case "Cold":        return "~g~"+n+":~s~ \"Efficient. Fine.\"";
                case "Playful":     return "~g~"+n+":~s~ \"Ha! Yes. Absolutely yes.\"";
                case "Gold Digger": return "~g~"+n+":~s~ \"Bold. Make it worth my time.\"";
                case "Party Girl":  return "~g~"+n+":~s~ \"Yes! That's what I'm talking about!\"";
                case "Needy":       return "~g~"+n+":~s~ \"...Yes. Don't make me wait.\"";
                case "Aggressive":  return "~g~"+n+":~s~ \"Now we're talking.\"";
                case "Sarcastic":   return "~g~"+n+":~s~ \"Real smooth. Yeah, sure.\"";
                case "Mysterious":  return "~g~"+n+":~s~ \"...Yeah.\"";
                case "Chaotic":     return "~g~"+n+":~s~ \"YES. Let's go let's go let's go!\"";
                case "Manipulative":return "~g~"+n+":~s~ \"Direct. I can work with that.\"";
                case "Classy":      return "~g~"+n+":~s~ \"No subtlety whatsoever. ...Fine.\"";
                case "Jealous":     return "~g~"+n+":~s~ \"You better only be asking me that.\"";
                case "Independent": return "~g~"+n+":~s~ \"At least you're honest. Yeah.\"";
                case "Street Smart":return "~g~"+n+":~s~ \"Blunt. Alright. But we're doing this my way.\"";
                case "Unstable":    return "~g~"+n+":~s~ \"YES. NOW. Let's GO.\"";
                default:            return "~g~"+n+":~s~ \"...Yeah. Let's do it.\"";
            }
            // lineIdx == 0: "Stay with me tonight." / "Come with me for a bit."
            switch (personality)
            {
                case "Dominant":    return "~g~"+n+":~s~ \"Lead the way. But I'm in charge.\"";
                case "Shy":         return "~g~"+n+":~s~ \"...Okay. Just... stay close.\"";
                case "Flirty":      return "~g~"+n+":~s~ \"I thought you'd never ask.\"";
                case "Sweet":       return "~g~"+n+":~s~ \"I'd really like that.\"";
                case "Romantic":    return "~g~"+n+":~s~ \"I've been waiting for you to say that.\"";
                case "Cold":        return "~g~"+n+":~s~ \"Fine. Don't make it a whole thing.\"";
                case "Playful":     return "~g~"+n+":~s~ \"Ooh, yes. Let's go.\"";
                case "Gold Digger": { int _hr = Function.Call<int>(Hash.GET_CLOCK_HOURS); return (_hr >= 21 || _hr < 3) ? "~g~"+n+":~s~ \"Sure. Make it worth the night.\"" : "~g~"+n+":~s~ \"Sure. Make it worth my time.\""; }
                case "Party Girl":  return "~g~"+n+":~s~ \"Hell yeah, let's get out of here.\"";
                case "Needy":       return "~g~"+n+":~s~ \"Yes... please don't let go.\"";
                case "Aggressive":  return "~g~"+n+":~s~ \"Finally. Move.\"";
                case "Sarcastic":   return "~g~"+n+":~s~ \"Sure, beats standing here.\"";
                case "Mysterious":  return "~g~"+n+":~s~ \"...Come on then.\"";
                case "Chaotic":     return "~g~"+n+":~s~ \"Yes! Let's go right now!\"";
                case "Manipulative":return "~g~"+n+":~s~ \"I'll allow it. For now.\"";
                case "Classy":      return "~g~"+n+":~s~ \"Alright. But I have expectations.\"";
                case "Jealous":     return "~g~"+n+":~s~ \"Only if it's just us.\"";
                case "Independent": return "~g~"+n+":~s~ \"On my terms. Understood?\"";
                case "Street Smart":return "~g~"+n+":~s~ \"Alright. Keep it discreet.\"";
                case "Unstable":    return "~g~"+n+":~s~ \"Yes! Yes! Don't change your mind!\"";
                default:            return "~g~"+n+":~s~ \"...Alright. Let's go.\"";
            }
        }

        private static string GetEscalateRejectLine(string name, string personality, int lineIdx)
        {
            string n = name;
            if (lineIdx == 1) // "Mind if I... come closer?.."
            switch (personality)
            {
                case "Dominant":    return "~r~"+n+":~s~ \"You can want. Doesn't mean you get.\"";
                case "Shy":         return "~r~"+n+":~s~ \"I'd rather keep some space right now.\"";
                case "Cold":        return "~r~"+n+":~s~ \"Yes, I mind. Keep your distance.\"";
                case "Sweet":       return "~r~"+n+":~s~ \"I'm not quite comfortable with that yet.\"";
                case "Romantic":    return "~r~"+n+":~s~ \"Not yet. We're not there.\"";
                case "Sarcastic":   return "~r~"+n+":~s~ \"Yes, I do mind. Nice question though.\"";
                case "Gold Digger": return "~r~"+n+":~s~ \"Closeness like that costs something.\"";
                case "Independent": return "~r~"+n+":~s~ \"I prefer my space, thanks.\"";
                case "Classy":      return "~r~"+n+":~s~ \"That's a bit forward.\"";
                case "Jealous":     return "~r~"+n+":~s~ \"I don't want that right now.\"";
                case "Needy":       return "~r~"+n+":~s~ \"I want to but... I need to trust you first.\"";
                case "Aggressive":  return "~r~"+n+":~s~ \"Back up.\"";
                case "Flirty":      return "~r~"+n+":~s~ \"Not right now.\"";
                case "Playful":     return "~r~"+n+":~s~ \"Hmm... not yet.\"";
                case "Chaotic":     return "~r~"+n+":~s~ \"No! Bad timing!\"";
                case "Manipulative":return "~r~"+n+":~s~ \"Closer isn't a right. It's earned.\"";
                case "Party Girl":  return "~r~"+n+":~s~ \"Maybe later.\"";
                case "Mysterious":  return "~r~"+n+":~s~ \"No.\"";
                case "Street Smart":return "~r~"+n+":~s~ \"Keep your distance.\"";
                case "Unstable":    return "~r~"+n+":~s~ \"Don't come near me right now!\"";
                default:            return "~r~"+n+":~s~ \"Give me some space.\"";
            }
            if (lineIdx == 2) // "Wanna fuck?"
            switch (personality)
            {
                case "Dominant":    return "~r~"+n+":~s~ \"You haven't earned that.\"";
                case "Shy":         return "~r~"+n+":~s~ \"You can't just... say that. No.\"";
                case "Cold":        return "~r~"+n+":~s~ \"No.\"";
                case "Sweet":       return "~r~"+n+":~s~ \"That's... a lot. No thank you.\"";
                case "Romantic":    return "~r~"+n+":~s~ \"That's not how I want this to go.\"";
                case "Sarcastic":   return "~r~"+n+":~s~ \"Wow. No.\"";
                case "Gold Digger": return "~r~"+n+":~s~ \"Not for free.\"";
                case "Independent": return "~r~"+n+":~s~ \"That's not how this goes.\"";
                case "Classy":      return "~r~"+n+":~s~ \"Absolutely not.\"";
                case "Jealous":     return "~r~"+n+":~s~ \"Don't talk to me like that.\"";
                case "Needy":       return "~r~"+n+":~s~ \"Not like this. Ask me properly.\"";
                case "Aggressive":  return "~r~"+n+":~s~ \"Watch your mouth.\"";
                case "Flirty":      return "~r~"+n+":~s~ \"You could've said that better. Try again.\"";
                case "Playful":     return "~r~"+n+":~s~ \"Points for boldness. Still no.\"";
                case "Chaotic":     return "~r~"+n+":~s~ \"No! Bad! Wrong!\"";
                case "Manipulative":return "~r~"+n+":~s~ \"That's not how you get what you want.\"";
                case "Party Girl":  return "~r~"+n+":~s~ \"Even I have standards. No.\"";
                case "Mysterious":  return "~r~"+n+":~s~ \"No.\"";
                case "Street Smart":return "~r~"+n+":~s~ \"That how you talk to women?\"";
                case "Unstable":    return "~r~"+n+":~s~ \"HOW DARE YOU. ...maybe later. BUT NO.\"";
                default:            return "~r~"+n+":~s~ \"Watch yourself.\"";
            }
            // lineIdx == 0: "Stay with me tonight." / "Come with me for a bit."
            switch (personality)
            {
                case "Dominant":    { int _hr0 = Function.Call<int>(Hash.GET_CLOCK_HOURS); bool _nt0 = _hr0 >= 21 || _hr0 < 3; return "~r~"+n+":~s~ " + (_nt0 ? "\"You haven't earned a night with me.\"" : "\"You haven't earned that with me.\""); }
                case "Shy":         return "~r~"+n+":~s~ \"I... I'm not ready for that.\"";
                case "Cold":        { int _hr0 = Function.Call<int>(Hash.GET_CLOCK_HOURS); bool _nt0 = _hr0 >= 21 || _hr0 < 3; return "~r~"+n+":~s~ " + (_nt0 ? "\"That's not happening tonight.\"" : "\"That's not happening.\""); }
                case "Sweet":       return "~r~"+n+":~s~ \"I like you, but I'm not there yet.\"";
                case "Romantic":    return "~r~"+n+":~s~ \"I need more between us before that.\"";
                case "Sarcastic":   return "~r~"+n+":~s~ \"Charming offer. Hard pass.\"";
                case "Gold Digger": { int _hr0 = Function.Call<int>(Hash.GET_CLOCK_HOURS); bool _nt0 = _hr0 >= 21 || _hr0 < 3; return "~r~"+n+":~s~ " + (_nt0 ? "\"A night costs more than just asking.\"" : "\"That costs more than just asking.\""); }
                case "Independent": return "~r~"+n+":~s~ \"I don't do that on your timeline.\"";
                case "Classy":      return "~r~"+n+":~s~ \"Not how this works.\"";
                case "Jealous":     return "~r~"+n+":~s~ \"You better not be asking anyone else this.\"";
                case "Needy":       return "~r~"+n+":~s~ \"I want to... but I need to know you mean it.\"";
                case "Aggressive":  { int _hr0 = Function.Call<int>(Hash.GET_CLOCK_HOURS); bool _nt0 = _hr0 >= 21 || _hr0 < 3; return "~r~"+n+":~s~ " + (_nt0 ? "\"Not tonight.\"" : "\"Not a chance.\""); }
                case "Flirty":      return "~r~"+n+":~s~ \"Maybe later. Not yet.\"";
                case "Playful":     return "~r~"+n+":~s~ \"Mmm... not quite yet.\"";
                case "Chaotic":     return "~r~"+n+":~s~ \"Wrong vibe right now.\"";
                case "Manipulative":{ int _hr0 = Function.Call<int>(Hash.GET_CLOCK_HOURS); bool _nt0 = _hr0 >= 21 || _hr0 < 3; return "~r~"+n+":~s~ " + (_nt0 ? "\"Earn the night first.\"" : "\"Earn it first.\""); }
                case "Party Girl":  { int _hr0 = Function.Call<int>(Hash.GET_CLOCK_HOURS); bool _nt0 = _hr0 >= 21 || _hr0 < 3; return "~r~"+n+":~s~ " + (_nt0 ? "\"Not tonight. Ask me after another round.\"" : "\"Not right now. Ask me after another round.\""); }
                case "Mysterious":  return "~r~"+n+":~s~ \"Not yet.\"";
                case "Street Smart":return "~r~"+n+":~s~ \"I don't know you well enough for that.\"";
                case "Unstable":    return "~r~"+n+":~s~ \"No! Don't push me!\"";
                default:            { int _hr0 = Function.Call<int>(Hash.GET_CLOCK_HOURS); bool _nt0 = _hr0 >= 21 || _hr0 < 3; return "~r~"+n+":~s~ " + (_nt0 ? "\"Not tonight.\"" : "\"Not a chance.\""); }
            }
        }

        private static string[] GetEscalatePermaRejectLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Dominant":    return new[]{ "~r~"+n+":~s~ \"You don't get a third chance with me.\"", "~r~"+n+":~s~ \"I said no. Now I'm done talking to you.\"" };
                case "Shy":         return new[]{ "~r~"+n+":~s~ \"Please... just leave me alone.\"", "~r~"+n+":~s~ \"I can't do this. I'm going.\"" };
                case "Cold":        { int _hr0 = Function.Call<int>(Hash.GET_CLOCK_HOURS); bool _nt0 = _hr0 >= 21 || _hr0 < 3; return new[]{ "~r~"+n+":~s~ \"We're done here.\"", "~r~"+n+":~s~ " + (_nt0 ? "\"Don't come near me again tonight.\"" : "\"Don't come near me again.\"") }; }
                case "Flirty":      return new[]{ "~r~"+n+":~s~ \"Okay, you ruined it. I'm out.\"", "~r~"+n+":~s~ \"Too pushy. Bye.\"" };
                case "Sweet":       return new[]{ "~r~"+n+":~s~ \"I was starting to like you... don't do this.\"", "~r~"+n+":~s~ \"You're making me uncomfortable. I'm leaving.\"" };
                case "Romantic":    return new[]{ "~r~"+n+":~s~ \"You just killed whatever this was.\"", "~r~"+n+":~s~ \"I wanted this to mean something. You ruined it.\"" };
                case "Playful":     return new[]{ "~r~"+n+":~s~ \"Okay, not funny anymore. I'm gone.\"", "~r~"+n+":~s~ \"You really can't take a hint.\"" };
                case "Gold Digger": return new[]{ "~r~"+n+":~s~ \"You're wasting my time.\"", "~r~"+n+":~s~ \"Not enough charm, not enough money. Bye.\"" };
                case "Party Girl":  return new[]{ "~r~"+n+":~s~ \"Even I have limits. You found mine.\"", "~r~"+n+":~s~ \"You're killing my vibe. I'm out.\"" };
                case "Needy":       return new[]{ "~r~"+n+":~s~ \"I trusted you and you keep pushing. I'm done.\"", "~r~"+n+":~s~ \"Why won't you just listen to me?!\"" };
                case "Aggressive":  return new[]{ "~r~"+n+":~s~ \"Keep it up and see what happens.\"", "~r~"+n+":~s~ \"Get away from me.\"" };
                case "Sarcastic":   return new[]{ "~r~"+n+":~s~ \"Wow. Truly breathtaking persistence. No.\"", "~r~"+n+":~s~ \"Let me spell it out — N. O. We done?\"" };
                case "Mysterious":  return new[]{ "~r~"+n+":~s~ \"...(she just walks away)\"", "~r~"+n+":~s~ \"...\"" };
                case "Chaotic":     return new[]{ "~r~"+n+":~s~ \"NOPE. Nope nope nope. BYE.\"", "~r~"+n+":~s~ \"That's it! I'm out of here!\"" };
                case "Manipulative":return new[]{ "~r~"+n+":~s~ \"You don't know how to play this game. Goodbye.\"", "~r~"+n+":~s~ \"You had a shot. You wasted it.\"" };
                case "Classy":      return new[]{ "~r~"+n+":~s~ \"I won't be spoken to like this. We're finished.\"", "~r~"+n+":~s~ \"Absolutely not. Don't contact me again.\"" };
                case "Jealous":     return new[]{ "~r~"+n+":~s~ \"You have no idea what you just threw away.\"", "~r~"+n+":~s~ \"Done. I'm done. Don't follow me.\"" };
                case "Independent": return new[]{ "~r~"+n+":~s~ \"I said no. That means no. Goodbye.\"", "~r~"+n+":~s~ \"You don't respect me. We're done.\"" };
                case "Street Smart":return new[]{ "~r~"+n+":~s~ \"I see what you are. Walk away.\"", "~r~"+n+":~s~ \"Don't push your luck. I'm leaving.\"" };
                case "Unstable":    return new[]{ "~r~"+n+":~s~ \"STOP IT! I'm GOING!\"", "~r~"+n+":~s~ \"Why do you KEEP doing this?! GET AWAY FROM ME!\"" };
                default:            return new[]{ "~r~"+n+":~s~ \"I said no. We're done.\"", "~r~"+n+":~s~ \"Don't ask me again.\"" };
            }
        }

        private static string[] GetNoBJKnownLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~r~"+n+":~s~ \"Please don't ask me that again.\"", "~r~"+n+":~s~ \"I said no. I meant it.\"", "~r~"+n+":~s~ \"I'm not comfortable with that. You know that.\"" };
                case "Sweet":       return new string[] { "~r~"+n+":~s~ \"I told you before, that's not for me.\"", "~r~"+n+":~s~ \"Please don't push on this.\"", "~r~"+n+":~s~ \"You know I don't want that.\"" };
                case "Romantic":    return new string[] { "~r~"+n+":~s~ \"That's not something I want. I've said it.\"", "~r~"+n+":~s~ \"Please respect that. I've been clear.\"", "~r~"+n+":~s~ \"Don't make me say it again.\"" };
                case "Needy":       return new string[] { "~r~"+n+":~s~ \"Why do you keep asking? I told you.\"", "~r~"+n+":~s~ \"Please just drop it. I don't want that.\"", "~r~"+n+":~s~ \"Stop asking or I'll get upset.\"" };
                case "Flirty":      return new string[] { "~r~"+n+":~s~ \"That's still a no from me.\"", "~r~"+n+":~s~ \"Cute try. Still not doing that.\"", "~r~"+n+":~s~ \"I'm into a lot of things. That's not one of them.\"" };
                case "Playful":     return new string[] { "~r~"+n+":~s~ \"Ha, still no. Give it up.\"", "~r~"+n+":~s~ \"You know I don't do that. Stop asking.\"", "~r~"+n+":~s~ \"Not a chance. Nice try though.\"" };
                case "Party Girl":  return new string[] { "~r~"+n+":~s~ \"I already told you — not my thing.\"", "~r~"+n+":~s~ \"Still no. Drop it.\"", "~r~"+n+":~s~ \"That's a hard limit. Stop pushing.\"" };
                case "Sarcastic":   return new string[] { "~r~"+n+":~s~ \"You know I'm not into that. Don't push it.\"", "~r~"+n+":~s~ \"I told you. That's not my thing.\"", "~r~"+n+":~s~ \"No. We talked about this.\"" };
                case "Cold":        return new string[] { "~r~"+n+":~s~ \"No. Final answer.\"", "~r~"+n+":~s~ \"We've been through this.\"", "~r~"+n+":~s~ \"Don't ask again.\"" };
                case "Independent": return new string[] { "~r~"+n+":~s~ \"I said no. That doesn't change.\"", "~r~"+n+":~s~ \"My answer stays the same.\"", "~r~"+n+":~s~ \"You know where I stand on this.\"" };
                case "Classy":      return new string[] { "~r~"+n+":~s~ \"I've made my position clear on that.\"", "~r~"+n+":~s~ \"That remains off the table.\"", "~r~"+n+":~s~ \"We've discussed this. The answer hasn't changed.\"" };
                case "Gold Digger": return new string[] { "~r~"+n+":~s~ \"That's not in my menu. End of story.\"", "~r~"+n+":~s~ \"You already know I won't do that.\"", "~r~"+n+":~s~ \"Still not happening. Move on.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"You already know the answer. Don't test me.\"", "~r~"+n+":~s~ \"That won't work. I've told you before.\"", "~r~"+n+":~s~ \"Still no. And pushing won't change that.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"You know I don't do that. Don't push it.\"", "~r~"+n+":~s~ \"I was clear. I'm still clear.\"", "~r~"+n+":~s~ \"That line doesn't move.\"" };
                case "Mysterious":  return new string[] { "~r~"+n+":~s~ \"That answer hasn't changed.\"", "~r~"+n+":~s~ \"Still no.\"", "~r~"+n+":~s~ \"Don't ask me that again.\"" };
                case "Jealous":     return new string[] { "~r~"+n+":~s~ \"I told you already. Don't make me repeat myself.\"", "~r~"+n+":~s~ \"You keep asking like I'll change my mind. I won't.\"", "~r~"+n+":~s~ \"Still no. And I don't appreciate being pushed.\"" };
                case "Dominant":    return new string[] { "~r~"+n+":~s~ \"I said no. My word is final.\"", "~r~"+n+":~s~ \"Don't push limits I've already set.\"", "~r~"+n+":~s~ \"We've covered this. No.\"" };
                case "Aggressive":  return new string[] { "~r~"+n+":~s~ \"I already said no. Back off.\"", "~r~"+n+":~s~ \"Don't make me say it again.\"", "~r~"+n+":~s~ \"I told you. It's not happening.\"" };
                case "Chaotic":     return new string[] { "~r~"+n+":~s~ \"NO. STILL NO. How many times?!\"", "~r~"+n+":~s~ \"I literally told you! Still a no!\"", "~r~"+n+":~s~ \"Wow. Still no. Wow.\"" };
                case "Unstable":    return new string[] { "~r~"+n+":~s~ \"Why do you keep asking?! I told you NO!\"", "~r~"+n+":~s~ \"Stop pushing me on this!\"", "~r~"+n+":~s~ \"I can't believe you're asking again. No!\"" };
                default:            return new string[] { "~r~"+n+":~s~ \"You know I'm not into that. Don't push it.\"", "~r~"+n+":~s~ \"I told you. That's not my thing.\"", "~r~"+n+":~s~ \"No. We talked about this.\"" };
            }
        }

        private static string[] GetNoBJNewLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~r~"+n+":~s~ \"Um... I'd rather not do that.\"", "~r~"+n+":~s~ \"That's a bit much for me.\"", "~r~"+n+":~s~ \"I'm not really comfortable with that.\"" };
                case "Sweet":       return new string[] { "~r~"+n+":~s~ \"That's not really my thing.\"", "~r~"+n+":~s~ \"I'd rather skip that part.\"", "~r~"+n+":~s~ \"I don't really want to do that.\"" };
                case "Romantic":    return new string[] { "~r~"+n+":~s~ \"That's not something I enjoy. Sorry.\"", "~r~"+n+":~s~ \"I'd rather we didn't.\"", "~r~"+n+":~s~ \"Not that. Something else.\"" };
                case "Needy":       return new string[] { "~r~"+n+":~s~ \"That makes me uncomfortable. Please don't.\"", "~r~"+n+":~s~ \"I really don't want to do that.\"", "~r~"+n+":~s~ \"Can we do something else? Please?\"" };
                case "Flirty":      return new string[] { "~r~"+n+":~s~ \"Ha, that's not really my scene.\"", "~r~"+n+":~s~ \"I'd rather not. Not into it.\"", "~r~"+n+":~s~ \"I've got limits too, you know.\"" };
                case "Playful":     return new string[] { "~r~"+n+":~s~ \"Hm, nah. Not that.\"", "~r~"+n+":~s~ \"I'd rather not, actually.\"", "~r~"+n+":~s~ \"Not that one. Something else.\"" };
                case "Party Girl":  return new string[] { "~r~"+n+":~s~ \"Eh, not really into that.\"", "~r~"+n+":~s~ \"I'd skip that one.\"", "~r~"+n+":~s~ \"Not my vibe.\"" };
                case "Sarcastic":   return new string[] { "~r~"+n+":~s~ \"That's... not really my thing.\"", "~r~"+n+":~s~ \"I'd rather not.\"", "~r~"+n+":~s~ \"No, not that.\"" };
                case "Cold":        return new string[] { "~r~"+n+":~s~ \"Not that.\"", "~r~"+n+":~s~ \"Skip it.\"", "~r~"+n+":~s~ \"No.\"" };
                case "Independent": return new string[] { "~r~"+n+":~s~ \"Not really for me.\"", "~r~"+n+":~s~ \"I'll pass on that.\"", "~r~"+n+":~s~ \"That's not something I want to do.\"" };
                case "Classy":      return new string[] { "~r~"+n+":~s~ \"That's not something I do.\"", "~r~"+n+":~s~ \"I'd prefer to skip that.\"", "~r~"+n+":~s~ \"Not that, please.\"" };
                case "Gold Digger": return new string[] { "~r~"+n+":~s~ \"That's not in my menu, actually.\"", "~r~"+n+":~s~ \"Not that. I don't do that.\"", "~r~"+n+":~s~ \"I'd rather not.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"That's not what I'm offering.\"", "~r~"+n+":~s~ \"Not that. Choose something else.\"", "~r~"+n+":~s~ \"I'd rather not.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"Nah, I'm not doing that.\"", "~r~"+n+":~s~ \"That's not happening.\"", "~r~"+n+":~s~ \"I don't do that.\"" };
                case "Mysterious":  return new string[] { "~r~"+n+":~s~ \"Not that.\"", "~r~"+n+":~s~ \"I'd rather not.\"", "~r~"+n+":~s~ \"No, not that.\"" };
                case "Jealous":     return new string[] { "~r~"+n+":~s~ \"I'm not into that. Don't push it.\"", "~r~"+n+":~s~ \"That's not something I want to do.\"", "~r~"+n+":~s~ \"No. Pick something else.\"" };
                case "Dominant":    return new string[] { "~r~"+n+":~s~ \"That's not what's happening.\"", "~r~"+n+":~s~ \"I don't do that.\"", "~r~"+n+":~s~ \"Not that. I decide.\"" };
                case "Aggressive":  return new string[] { "~r~"+n+":~s~ \"No. Not doing that.\"", "~r~"+n+":~s~ \"That's a no.\"", "~r~"+n+":~s~ \"Pick something else.\"" };
                case "Chaotic":     return new string[] { "~r~"+n+":~s~ \"Ew, no. Not that one!\"", "~r~"+n+":~s~ \"Ha — nope! Hard pass!\"", "~r~"+n+":~s~ \"Not that. Literally anything else.\"" };
                case "Unstable":    return new string[] { "~r~"+n+":~s~ \"No! I don't want that!\"", "~r~"+n+":~s~ \"Please don't ask me that.\"", "~r~"+n+":~s~ \"I can't do that. I just can't.\"" };
                default:            return new string[] { "~r~"+n+":~s~ \"That's... not really my thing.\"", "~r~"+n+":~s~ \"I'd rather not.\"", "~r~"+n+":~s~ \"No, not that.\"" };
            }
        }

        private static string[] GetNoRoughKnownLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~r~"+n+":~s~ \"Please. I told you I don't like it rough.\"", "~r~"+n+":~s~ \"I already said no to that.\"", "~r~"+n+":~s~ \"You know I don't want that.\"" };
                case "Sweet":       return new string[] { "~r~"+n+":~s~ \"I've said this before — not rough. Please.\"", "~r~"+n+":~s~ \"You know that's not what I want.\"", "~r~"+n+":~s~ \"Please don't push on this.\"" };
                case "Romantic":    return new string[] { "~r~"+n+":~s~ \"That's not how I want this. I've said it.\"", "~r~"+n+":~s~ \"Please respect what I've told you.\"", "~r~"+n+":~s~ \"I want it gentle. You know that.\"" };
                case "Needy":       return new string[] { "~r~"+n+":~s~ \"Why do you keep trying that? I said no.\"", "~r~"+n+":~s~ \"I don't like it rough. Please stop.\"", "~r~"+n+":~s~ \"You're going to upset me if you keep pushing.\"" };
                case "Flirty":      return new string[] { "~r~"+n+":~s~ \"I already told you — not rough. I mean it.\"", "~r~"+n+":~s~ \"That's still a no. And it's staying a no.\"", "~r~"+n+":~s~ \"Cute try. I don't want it rough.\"" };
                case "Playful":     return new string[] { "~r~"+n+":~s~ \"Ha, still no. Not rough. You know this.\"", "~r~"+n+":~s~ \"Still not into that. Give up already.\"", "~r~"+n+":~s~ \"I'm not rough. You know that.\"" };
                case "Party Girl":  return new string[] { "~r~"+n+":~s~ \"I already told you — I'm not into it rough.\"", "~r~"+n+":~s~ \"Hard limit. Still.\"", "~r~"+n+":~s~ \"Nope. We've talked. Still no.\"" };
                case "Sarcastic":   return new string[] { "~r~"+n+":~s~ \"Not rough. You know that's not what I want.\"", "~r~"+n+":~s~ \"I already told you — I'm not into it rough.\"", "~r~"+n+":~s~ \"That's a no. Don't ask again.\"" };
                case "Cold":        return new string[] { "~r~"+n+":~s~ \"No rough. Final.\"", "~r~"+n+":~s~ \"Already said no to this.\"", "~r~"+n+":~s~ \"Don't ask again.\"" };
                case "Independent": return new string[] { "~r~"+n+":~s~ \"I said no rough. That stands.\"", "~r~"+n+":~s~ \"My boundary. It doesn't move.\"", "~r~"+n+":~s~ \"You know this already.\"" };
                case "Classy":      return new string[] { "~r~"+n+":~s~ \"I've been clear about that. Not rough.\"", "~r~"+n+":~s~ \"That remains off the table.\"", "~r~"+n+":~s~ \"We've discussed this. The answer is no.\"" };
                case "Gold Digger": return new string[] { "~r~"+n+":~s~ \"I don't do rough. You know that.\"", "~r~"+n+":~s~ \"Still no. That's not changing.\"", "~r~"+n+":~s~ \"Not rough. End of story.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"You know that won't work. Still no.\"", "~r~"+n+":~s~ \"I've told you. Pushing won't change it.\"", "~r~"+n+":~s~ \"Still no. And now I'm annoyed.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"I was clear on this. Still am.\"", "~r~"+n+":~s~ \"That line doesn't move.\"", "~r~"+n+":~s~ \"You know I don't do rough. Stop.\"" };
                case "Mysterious":  return new string[] { "~r~"+n+":~s~ \"Still no.\"", "~r~"+n+":~s~ \"That hasn't changed.\"", "~r~"+n+":~s~ \"Don't ask me that again.\"" };
                case "Jealous":     return new string[] { "~r~"+n+":~s~ \"I told you already. No rough.\"", "~r~"+n+":~s~ \"Stop pushing this. I've been clear.\"", "~r~"+n+":~s~ \"Still no. I don't appreciate being pushed.\"" };
                case "Dominant":    return new string[] { "~r~"+n+":~s~ \"I set the pace. Not you. And I said no rough.\"", "~r~"+n+":~s~ \"Don't push limits I've set. No rough.\"", "~r~"+n+":~s~ \"We've covered this. No.\"" };
                case "Aggressive":  return new string[] { "~r~"+n+":~s~ \"I said no rough. Back off.\"", "~r~"+n+":~s~ \"Don't push me on this. I told you.\"", "~r~"+n+":~s~ \"Not rough. Last time I say it.\"" };
                case "Chaotic":     return new string[] { "~r~"+n+":~s~ \"NO ROUGH. I'VE SAID IT. MULTIPLE TIMES.\"", "~r~"+n+":~s~ \"STILL NO. How hard is that?!\"", "~r~"+n+":~s~ \"Literally told you. Still no. Wow.\"" };
                case "Unstable":    return new string[] { "~r~"+n+":~s~ \"WHY do you keep asking?! NO rough!\"", "~r~"+n+":~s~ \"Stop pushing me on this! I said NO!\"", "~r~"+n+":~s~ \"I told you and you're still asking?!\"" };
                default:            return new string[] { "~r~"+n+":~s~ \"Not rough. You know that's not what I want.\"", "~r~"+n+":~s~ \"I already told you — I'm not into it rough.\"", "~r~"+n+":~s~ \"That's a no. Don't ask again.\"" };
            }
        }

        private static string[] GetNoRoughNewLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~r~"+n+":~s~ \"I don't really want it rough...\"", "~r~"+n+":~s~ \"That's a bit scary for me.\"", "~r~"+n+":~s~ \"Can we keep it gentle?\"" };
                case "Sweet":       return new string[] { "~r~"+n+":~s~ \"I'd rather it stay gentle.\"", "~r~"+n+":~s~ \"Not rough, please.\"", "~r~"+n+":~s~ \"I'm not really into that.\"" };
                case "Romantic":    return new string[] { "~r~"+n+":~s~ \"That's not how I want this to feel.\"", "~r~"+n+":~s~ \"I want it soft, not rough.\"", "~r~"+n+":~s~ \"Not like that.\"" };
                case "Needy":       return new string[] { "~r~"+n+":~s~ \"That makes me uncomfortable. Not rough.\"", "~r~"+n+":~s~ \"Please, not rough. I don't like it.\"", "~r~"+n+":~s~ \"Can we not do rough? Please?\"" };
                case "Flirty":      return new string[] { "~r~"+n+":~s~ \"Hmm, I'm not really into rough.\"", "~r~"+n+":~s~ \"That's not quite my thing.\"", "~r~"+n+":~s~ \"I'd rather keep it fun, not rough.\"" };
                case "Playful":     return new string[] { "~r~"+n+":~s~ \"Eh, I'm not really into rough.\"", "~r~"+n+":~s~ \"Let's keep it chill.\"", "~r~"+n+":~s~ \"Not that rough stuff.\"" };
                case "Party Girl":  return new string[] { "~r~"+n+":~s~ \"Nah, not rough for me.\"", "~r~"+n+":~s~ \"I like fun, not rough.\"", "~r~"+n+":~s~ \"Not my vibe.\"" };
                case "Sarcastic":   return new string[] { "~r~"+n+":~s~ \"I don't really want it rough.\"", "~r~"+n+":~s~ \"Not like that.\"", "~r~"+n+":~s~ \"That's not what I'm into.\"" };
                case "Cold":        return new string[] { "~r~"+n+":~s~ \"Not rough.\"", "~r~"+n+":~s~ \"Skip it.\"", "~r~"+n+":~s~ \"No.\"" };
                case "Independent": return new string[] { "~r~"+n+":~s~ \"I prefer to keep it on my terms. Not rough.\"", "~r~"+n+":~s~ \"Not really for me.\"", "~r~"+n+":~s~ \"I'd pass on that.\"" };
                case "Classy":      return new string[] { "~r~"+n+":~s~ \"I prefer something a little more refined.\"", "~r~"+n+":~s~ \"That's not my preference.\"", "~r~"+n+":~s~ \"Not that, please.\"" };
                case "Gold Digger": return new string[] { "~r~"+n+":~s~ \"That's not what I signed up for.\"", "~r~"+n+":~s~ \"I don't do rough.\"", "~r~"+n+":~s~ \"Not happening.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"That's not what I'm offering.\"", "~r~"+n+":~s~ \"I don't do rough. Not interested.\"", "~r~"+n+":~s~ \"Choose something else.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"Nah. I don't do rough.\"", "~r~"+n+":~s~ \"That's not happening.\"", "~r~"+n+":~s~ \"Not that.\"" };
                case "Mysterious":  return new string[] { "~r~"+n+":~s~ \"Not rough.\"", "~r~"+n+":~s~ \"That's not what I want.\"", "~r~"+n+":~s~ \"No.\"" };
                case "Jealous":     return new string[] { "~r~"+n+":~s~ \"I'm not into rough. Not with anyone.\"", "~r~"+n+":~s~ \"That's not what I want.\"", "~r~"+n+":~s~ \"No rough.\"" };
                case "Dominant":    return new string[] { "~r~"+n+":~s~ \"I lead. And I don't want rough.\"", "~r~"+n+":~s~ \"Not rough. I decide.\"", "~r~"+n+":~s~ \"That's not what's happening.\"" };
                case "Aggressive":  return new string[] { "~r~"+n+":~s~ \"No. Not doing rough.\"", "~r~"+n+":~s~ \"That's a no.\"", "~r~"+n+":~s~ \"Pick something else.\"" };
                case "Chaotic":     return new string[] { "~r~"+n+":~s~ \"Oof — nope! Not rough!\"", "~r~"+n+":~s~ \"Ha — hard pass on that one!\"", "~r~"+n+":~s~ \"Not that! Anything else!\"" };
                case "Unstable":    return new string[] { "~r~"+n+":~s~ \"No! That scares me!\"", "~r~"+n+":~s~ \"Please not rough. Please.\"", "~r~"+n+":~s~ \"I can't do rough. I just can't.\"" };
                default:            return new string[] { "~r~"+n+":~s~ \"I don't really want it rough.\"", "~r~"+n+":~s~ \"Not like that.\"", "~r~"+n+":~s~ \"That's not what I'm into.\"" };
            }
        }

        private static string[] GetServiceUnavailableLines(string name, string personality)
        {
            string n = name;
            switch (personality)
            {
                case "Shy":         return new string[] { "~r~"+n+":~s~ \"I'm sorry, I can't do that.\"", "~r~"+n+":~s~ \"That's not something I'm okay with.\"" };
                case "Sweet":       return new string[] { "~r~"+n+":~s~ \"That's not really something I offer. Sorry!\"", "~r~"+n+":~s~ \"I can't do that one. Pick something else?\"" };
                case "Romantic":    return new string[] { "~r~"+n+":~s~ \"That's not something I do. I'm sorry.\"", "~r~"+n+":~s~ \"That's not for me. Choose something else.\"" };
                case "Needy":       return new string[] { "~r~"+n+":~s~ \"I can't do that... please pick something else?\"", "~r~"+n+":~s~ \"That's not something I do. Is that okay?\"" };
                case "Flirty":      return new string[] { "~r~"+n+":~s~ \"Ha, not that one. Try something else.\"", "~r~"+n+":~s~ \"That's off the menu, babe. Pick again.\"" };
                case "Playful":     return new string[] { "~r~"+n+":~s~ \"Nope, not that. Pick something fun.\"", "~r~"+n+":~s~ \"That one's a no. What else you got?\"" };
                case "Party Girl":  return new string[] { "~r~"+n+":~s~ \"Not that. I don't do that.\"", "~r~"+n+":~s~ \"Pick something else. That's off limits.\"" };
                case "Sarcastic":   return new string[] { "~r~"+n+":~s~ \"That's not something I do.\"", "~r~"+n+":~s~ \"I don't offer that. Pick something else.\"" };
                case "Cold":        return new string[] { "~r~"+n+":~s~ \"No. Not available.\"", "~r~"+n+":~s~ \"I don't do that. Move on.\"" };
                case "Independent": return new string[] { "~r~"+n+":~s~ \"That's not on my list.\"", "~r~"+n+":~s~ \"I don't offer that. Find something else.\"" };
                case "Classy":      return new string[] { "~r~"+n+":~s~ \"That's not something I provide.\"", "~r~"+n+":~s~ \"That's not on the table. Choose appropriately.\"" };
                case "Gold Digger": return new string[] { "~r~"+n+":~s~ \"That's not in my repertoire.\"", "~r~"+n+":~s~ \"I don't do that. Pick something I do.\"" };
                case "Manipulative":return new string[] { "~r~"+n+":~s~ \"That's not what I'm offering.\"", "~r~"+n+":~s~ \"Not available. Choose something else.\"" };
                case "Street Smart":return new string[] { "~r~"+n+":~s~ \"I don't do that. Simple.\"", "~r~"+n+":~s~ \"Not on my list. Pick again.\"" };
                case "Mysterious":  return new string[] { "~r~"+n+":~s~ \"That's not something I do.\"", "~r~"+n+":~s~ \"Not that. Something else.\"" };
                case "Jealous":     return new string[] { "~r~"+n+":~s~ \"I don't do that. Just so you know.\"", "~r~"+n+":~s~ \"That's not available. Pick something else.\"" };
                case "Dominant":    return new string[] { "~r~"+n+":~s~ \"That's not something I offer. My call.\"", "~r~"+n+":~s~ \"Off the table. Choose something I allow.\"" };
                case "Aggressive":  return new string[] { "~r~"+n+":~s~ \"No. I don't do that.\"", "~r~"+n+":~s~ \"Not happening. Pick something else.\"" };
                case "Chaotic":     return new string[] { "~r~"+n+":~s~ \"Ha! Nope. Not that one.\"", "~r~"+n+":~s~ \"Off limits! Pick literally anything else.\"" };
                case "Unstable":    return new string[] { "~r~"+n+":~s~ \"No! I can't do that!\"", "~r~"+n+":~s~ \"That's not something I do. Please don't ask.\"" };
                default:            return new string[] { "~r~"+n+":~s~ \"That's not something I do.\"", "~r~"+n+":~s~ \"I don't offer that. Pick something else.\"" };
            }
        }

        private static string[] GetCivilianNotHookerRejectLines(string name, string personality, int cluster)
        {
            string n = name;
            switch (cluster)
            {
                case 0: // Gentle approach
                    switch (personality)
                    {
                        case "Shy":          return new string[] { "~r~"+n+":~s~ \"Oh... I think you've got the wrong idea about me.\"", "~r~"+n+":~s~ \"I'm not... that's not what I do. Sorry.\"", "~r~"+n+":~s~ \"I'm flattered, but I'm not— no.\"" };
                        case "Sweet":        return new string[] { "~r~"+n+":~s~ \"Oh, I think there's been a misunderstanding.\"", "~r~"+n+":~s~ \"You're sweet but I'm not what you're looking for.\"", "~r~"+n+":~s~ \"I'm not that kind of girl, but thank you.\"" };
                        case "Romantic":     return new string[] { "~r~"+n+":~s~ \"I appreciate how you said that, but I'm not working.\"", "~r~"+n+":~s~ \"That was kind. I'm not a sex worker though.\"", "~r~"+n+":~s~ \"I'm not— no. But thank you.\"" };
                        case "Needy":        return new string[] { "~r~"+n+":~s~ \"Please don't think that about me...\"", "~r~"+n+":~s~ \"I'm not a hooker. I'm sorry if I gave that impression.\"", "~r~"+n+":~s~ \"That's not what I am. I just want to be clear.\"" };
                        case "Flirty":       return new string[] { "~r~"+n+":~s~ \"Ha, I'm flattered — but that's not what I am.\"", "~r~"+n+":~s~ \"Wrong girl, but nice approach.\"", "~r~"+n+":~s~ \"I think you've got the wrong read on me.\"" };
                        case "Playful":      return new string[] { "~r~"+n+":~s~ \"Ha, no! Wrong girl entirely.\"", "~r~"+n+":~s~ \"I appreciate the style but I'm not working.\"", "~r~"+n+":~s~ \"Nope! But A for effort.\"" };
                        case "Party Girl":   return new string[] { "~r~"+n+":~s~ \"Oh no, I'm not working. Just out here.\"", "~r~"+n+":~s~ \"Ha, I'm just partying. Not for sale.\"", "~r~"+n+":~s~ \"Wrong assumption, but I'll take the compliment.\"" };
                        case "Sarcastic":    return new string[] { "~r~"+n+":~s~ \"Wow. No. I'm really not.\"", "~r~"+n+":~s~ \"Cute. I'm still not a hooker though.\"", "~r~"+n+":~s~ \"Thanks for the creative introduction. Still no.\"" };
                        case "Cold":         return new string[] { "~r~"+n+":~s~ \"No. I'm not working. Move on.\"", "~r~"+n+":~s~ \"Wrong assumption. Leave.\"", "~r~"+n+":~s~ \"I'm not what you're looking for.\"" };
                        case "Independent":  return new string[] { "~r~"+n+":~s~ \"I think you've got the wrong idea about me.\"", "~r~"+n+":~s~ \"I'm not a hooker. Just so we're clear.\"", "~r~"+n+":~s~ \"That's not what I am. Move on.\"" };
                        case "Classy":       return new string[] { "~r~"+n+":~s~ \"I appreciate the politeness. I'm still not for hire.\"", "~r~"+n+":~s~ \"That was gracious, but I'm afraid you're mistaken.\"", "~r~"+n+":~s~ \"I'm not a sex worker. I hope that's clear.\"" };
                        case "Gold Digger":  return new string[] { "~r~"+n+":~s~ \"I'm flattered, but I don't operate that way.\"", "~r~"+n+":~s~ \"Nice approach. Still not a hooker.\"", "~r~"+n+":~s~ \"Wrong girl. I'm worth more than that.\"" };
                        case "Manipulative": return new string[] { "~r~"+n+":~s~ \"That was smooth. I'm still not working.\"", "~r~"+n+":~s~ \"Appreciate the effort. Not what I am.\"", "~r~"+n+":~s~ \"Interesting approach. Wrong girl entirely.\"" };
                        case "Street Smart": return new string[] { "~r~"+n+":~s~ \"Nice tone. Still the wrong call.\"", "~r~"+n+":~s~ \"I don't work like that. Just so we're clear.\"", "~r~"+n+":~s~ \"You got the wrong read on me.\"" };
                        case "Mysterious":   return new string[] { "~r~"+n+":~s~ \"I think you've misread the situation.\"", "~r~"+n+":~s~ \"That's not what I am.\"", "~r~"+n+":~s~ \"Wrong conclusion.\"" };
                        case "Jealous":      return new string[] { "~r~"+n+":~s~ \"I'm not working. I hope that's not what you assumed.\"", "~r~"+n+":~s~ \"That's not what I am. Just so we're clear.\"", "~r~"+n+":~s~ \"I'm not a hooker. Did someone say I was?\"" };
                        case "Dominant":     return new string[] { "~r~"+n+":~s~ \"I appreciate the approach. I'm not for sale.\"", "~r~"+n+":~s~ \"That was polite. The answer is still no.\"", "~r~"+n+":~s~ \"I don't work. We clear?\"" };
                        case "Aggressive":   return new string[] { "~r~"+n+":~s~ \"I'm not a hooker. Don't come near me.\"", "~r~"+n+":~s~ \"What? No. I don't do that. Back off.\"" };
                        case "Chaotic":      return new string[] { "~r~"+n+":~s~ \"Wait, WHAT? No! I'm just standing here!\"", "~r~"+n+":~s~ \"Ha — bold assumption. Completely wrong. Bye.\"", "~r~"+n+":~s~ \"What made you think THAT? No!\"" };
                        case "Unstable":     return new string[] { "~r~"+n+":~s~ \"No no no, that's not me.\"", "~r~"+n+":~s~ \"I'm not— please don't assume that.\"", "~r~"+n+":~s~ \"You've got it wrong. Please leave.\"" };
                        default:             return new string[] { "~r~"+n+":~s~ \"I think you've got the wrong idea about me.\"", "~r~"+n+":~s~ \"I'm not a hooker. Just so we're clear.\"", "~r~"+n+":~s~ \"That's not what I am. Move on.\"" };
                    }
                case 1: // Confident approach
                    switch (personality)
                    {
                        case "Shy":          return new string[] { "~r~"+n+":~s~ \"I... I'm not working. Please leave me alone.\"", "~r~"+n+":~s~ \"That's not— I'm not a sex worker. Sorry.\"" };
                        case "Sweet":        return new string[] { "~r~"+n+":~s~ \"I'm sorry, I think you've made a mistake.\"", "~r~"+n+":~s~ \"That's really not what I am. Sorry!\"" };
                        case "Romantic":     return new string[] { "~r~"+n+":~s~ \"I'm not for hire. I hope that doesn't offend you.\"", "~r~"+n+":~s~ \"That's not what I am. I'm sorry you thought so.\"" };
                        case "Needy":        return new string[] { "~r~"+n+":~s~ \"Please don't say that... I'm not working.\"", "~r~"+n+":~s~ \"I'm not a hooker. That really upset me.\"" };
                        case "Flirty":       return new string[] { "~r~"+n+":~s~ \"Bold move. Wrong girl though.\"", "~r~"+n+":~s~ \"I appreciate the confidence. Wrong person.\"", "~r~"+n+":~s~ \"Ha! Direct. Wrong. But direct.\"" };
                        case "Playful":      return new string[] { "~r~"+n+":~s~ \"Ha! Bold. Wrong. But bold.\"", "~r~"+n+":~s~ \"You've got the wrong girl but I like the energy.\"" };
                        case "Party Girl":   return new string[] { "~r~"+n+":~s~ \"Uh, no. Not a hooker. Nice try.\"", "~r~"+n+":~s~ \"Wrong girl, try the next block.\"", "~r~"+n+":~s~ \"Bold. Wrong. Move on.\"" };
                        case "Sarcastic":    return new string[] { "~r~"+n+":~s~ \"Wow, bold. And wrong.\"", "~r~"+n+":~s~ \"Confidence. Nice. I'm still not a hooker.\"", "~r~"+n+":~s~ \"You're decisive. You're also wrong.\"" };
                        case "Cold":         return new string[] { "~r~"+n+":~s~ \"Wrong assumption. I don't do that.\"", "~r~"+n+":~s~ \"I'm not a hooker. You've got the wrong girl.\"", "~r~"+n+":~s~ \"Not. For. Sale. Walk away.\"" };
                        case "Independent":  return new string[] { "~r~"+n+":~s~ \"I'm not a hooker. You've got the wrong girl.\"", "~r~"+n+":~s~ \"Wrong assumption. I don't do that.\"", "~r~"+n+":~s~ \"Not. For. Sale. Walk away.\"" };
                        case "Classy":       return new string[] { "~r~"+n+":~s~ \"I beg your pardon? I'm not for hire.\"", "~r~"+n+":~s~ \"That's quite an assumption. You're wrong.\"", "~r~"+n+":~s~ \"I'm not a sex worker. Adjust your approach.\"" };
                        case "Gold Digger":  return new string[] { "~r~"+n+":~s~ \"I'm flattered by the confidence, but no.\"", "~r~"+n+":~s~ \"Wrong girl. I don't work like that.\"", "~r~"+n+":~s~ \"Try harder elsewhere. I'm not for sale.\"" };
                        case "Manipulative": return new string[] { "~r~"+n+":~s~ \"Confident. Wrong. Impressive combo.\"", "~r~"+n+":~s~ \"I'm not a sex worker. Nice try though.\"", "~r~"+n+":~s~ \"You read me completely wrong.\"" };
                        case "Street Smart": return new string[] { "~r~"+n+":~s~ \"I'm not working. You read that wrong.\"", "~r~"+n+":~s~ \"Wrong assumption. Move along.\"", "~r~"+n+":~s~ \"That's not what I am. Keep walking.\"" };
                        case "Mysterious":   return new string[] { "~r~"+n+":~s~ \"Wrong read.\"", "~r~"+n+":~s~ \"I'm not working. Move on.\"", "~r~"+n+":~s~ \"Not what I am.\"" };
                        case "Jealous":      return new string[] { "~r~"+n+":~s~ \"No. I'm not a hooker. Don't assume that.\"", "~r~"+n+":~s~ \"You've got me wrong. I'm not working.\"", "~r~"+n+":~s~ \"Why would you even think that?\"" };
                        case "Dominant":     return new string[] { "~r~"+n+":~s~ \"I'm not for sale. That was presumptuous.\"", "~r~"+n+":~s~ \"Wrong call. I don't work.\"", "~r~"+n+":~s~ \"You assumed wrong. Walk away.\"" };
                        case "Aggressive":   return new string[] { "~r~"+n+":~s~ \"Are you serious? I'm not a hooker!\"", "~r~"+n+":~s~ \"Back off. Wrong girl entirely.\"", "~r~"+n+":~s~ \"Get away from me. I don't do that.\"" };
                        case "Chaotic":      return new string[] { "~r~"+n+":~s~ \"WHAT? No! Absolutely not!\"", "~r~"+n+":~s~ \"Ha — WRONG. Completely. Wow.\"", "~r~"+n+":~s~ \"That's honestly impressive how wrong you are.\"" };
                        case "Unstable":     return new string[] { "~r~"+n+":~s~ \"No! That's not what I am!\"", "~r~"+n+":~s~ \"How could you think that?!\"", "~r~"+n+":~s~ \"I'm not— just go away.\"" };
                        default:             return new string[] { "~r~"+n+":~s~ \"I'm not a hooker. You've got the wrong girl.\"", "~r~"+n+":~s~ \"Wrong assumption. I don't do that.\"", "~r~"+n+":~s~ \"Not. For. Sale. Walk away.\"" };
                    }
                case 2: // Cool approach
                    switch (personality)
                    {
                        case "Shy":          return new string[] { "~r~"+n+":~s~ \"I... think you've misunderstood.\"", "~r~"+n+":~s~ \"I'm not working. Please just go.\"" };
                        case "Sweet":        return new string[] { "~r~"+n+":~s~ \"Oh no, I think there's been a mix-up.\"", "~r~"+n+":~s~ \"I'm not what you're looking for. Sorry!\"" };
                        case "Romantic":     return new string[] { "~r~"+n+":~s~ \"That's a misunderstanding. I'm not working.\"", "~r~"+n+":~s~ \"I'm not a sex worker. Just to be clear.\"" };
                        case "Needy":        return new string[] { "~r~"+n+":~s~ \"I'm not... please don't assume that.\"", "~r~"+n+":~s~ \"That's not what I am. It makes me feel awful.\"" };
                        case "Flirty":       return new string[] { "~r~"+n+":~s~ \"Ha, creative. Wrong girl.\"", "~r~"+n+":~s~ \"I'm not working but I like how you asked.\"", "~r~"+n+":~s~ \"Wrong read but nice energy.\"" };
                        case "Playful":      return new string[] { "~r~"+n+":~s~ \"Oh wow, a real detective. Wrong though!\"", "~r~"+n+":~s~ \"Not a hooker! Good guess otherwise!\"", "~r~"+n+":~s~ \"Ha — completely wrong. Love the confidence.\"" };
                        case "Party Girl":   return new string[] { "~r~"+n+":~s~ \"Haha, no way. Not me.\"", "~r~"+n+":~s~ \"Wrong girl, totally.\"", "~r~"+n+":~s~ \"I'm just out here. Not working.\"" };
                        case "Sarcastic":    return new string[] { "~r~"+n+":~s~ \"Did you just assume I work the street? Really?\"", "~r~"+n+":~s~ \"I'm going to need you to reconsider what just happened here.\"" };
                        case "Cold":         return new string[] { "~r~"+n+":~s~ \"Wrong read. Completely wrong. Go.\"", "~r~"+n+":~s~ \"Did you just assume I'm a hooker?\"", "~r~"+n+":~s~ \"I'm not working. Not what I am. Leave.\"" };
                        case "Independent":  return new string[] { "~r~"+n+":~s~ \"I'm not working. Not what I am. Leave.\"", "~r~"+n+":~s~ \"Wrong read. Completely wrong. Go.\"", "~r~"+n+":~s~ \"Did you just assume I'm a hooker?\"" };
                        case "Classy":       return new string[] { "~r~"+n+":~s~ \"I'm sorry, did you just— no. I'm not for hire.\"", "~r~"+n+":~s~ \"That's quite the assumption. You're wrong.\"", "~r~"+n+":~s~ \"I'm not a sex worker. I suggest you move along.\"" };
                        case "Gold Digger":  return new string[] { "~r~"+n+":~s~ \"Wrong girl. I'm not for sale.\"", "~r~"+n+":~s~ \"Interesting assumption. Not correct.\"", "~r~"+n+":~s~ \"I don't work the street. Try again somewhere else.\"" };
                        case "Manipulative": return new string[] { "~r~"+n+":~s~ \"Interesting read. Wrong though.\"", "~r~"+n+":~s~ \"You thought you had me figured out. You don't.\"", "~r~"+n+":~s~ \"Cool approach. Wrong conclusion.\"" };
                        case "Street Smart": return new string[] { "~r~"+n+":~s~ \"Did you just assume I work the street? Really?\"", "~r~"+n+":~s~ \"I'm going to need you to reconsider what just happened here.\"" };
                        case "Mysterious":   return new string[] { "~r~"+n+":~s~ \"Wrong read.\"", "~r~"+n+":~s~ \"That's not what I am.\"", "~r~"+n+":~s~ \"Incorrect.\"" };
                        case "Jealous":      return new string[] { "~r~"+n+":~s~ \"You thought I was a hooker? Really?\"", "~r~"+n+":~s~ \"That's insulting. I'm not working.\"", "~r~"+n+":~s~ \"Did someone tell you that? Because they're wrong.\"" };
                        case "Dominant":     return new string[] { "~r~"+n+":~s~ \"Wrong assumption. I'm not for sale.\"", "~r~"+n+":~s~ \"You read me wrong. Walk away.\"", "~r~"+n+":~s~ \"That wasn't accurate. Move on.\"" };
                        case "Aggressive":   return new string[] { "~r~"+n+":~s~ \"Are you kidding me right now?\"", "~r~"+n+":~s~ \"Wrong girl. Back up.\"", "~r~"+n+":~s~ \"I don't work. Not even close.\"" };
                        case "Chaotic":      return new string[] { "~r~"+n+":~s~ \"OH WOW. Nope. Completely off. Wow.\"", "~r~"+n+":~s~ \"The audacity?! I love it. Still wrong.\"", "~r~"+n+":~s~ \"Ha! That's impressively incorrect.\"" };
                        case "Unstable":     return new string[] { "~r~"+n+":~s~ \"No! Don't— I'm not that!\"", "~r~"+n+":~s~ \"Why would you say that to me?!\"", "~r~"+n+":~s~ \"That's not what I am. Leave me alone.\"" };
                        default:             return new string[] { "~r~"+n+":~s~ \"Did you just assume I'm a hooker?\"", "~r~"+n+":~s~ \"I'm not working. Not what I am. Leave.\"", "~r~"+n+":~s~ \"Wrong read. Completely wrong. Go.\"" };
                    }
                case 3: // Flattering approach
                    switch (personality)
                    {
                        case "Shy":          return new string[] { "~r~"+n+":~s~ \"Thank you... but I'm not what you're looking for.\"", "~r~"+n+":~s~ \"That's sweet, but I'm not working.\"", "~r~"+n+":~s~ \"I'm flattered, really. I'm not a hooker though.\"" };
                        case "Sweet":        return new string[] { "~r~"+n+":~s~ \"Aw, that's really nice! I'm still not a hooker though.\"", "~r~"+n+":~s~ \"You're so sweet but I don't do that.\"", "~r~"+n+":~s~ \"Thank you! But I'm really not for sale.\"" };
                        case "Romantic":     return new string[] { "~r~"+n+":~s~ \"That means a lot, really. But I'm not working.\"", "~r~"+n+":~s~ \"You're kind. I'm still not for hire.\"", "~r~"+n+":~s~ \"I appreciate that. I'm not a sex worker though.\"" };
                        case "Needy":        return new string[] { "~r~"+n+":~s~ \"That's the nicest thing... but I'm not working.\"", "~r~"+n+":~s~ \"Thank you so much. I still can't do that.\"", "~r~"+n+":~s~ \"I really appreciate it but I'm not a hooker.\"" };
                        case "Flirty":       return new string[] { "~r~"+n+":~s~ \"Ooh, smooth. Still not a hooker.\"", "~r~"+n+":~s~ \"That was good. Wrong girl though.\"", "~r~"+n+":~s~ \"Nice. Still not for sale.\"" };
                        case "Playful":      return new string[] { "~r~"+n+":~s~ \"Ha! Well played. Still no.\"", "~r~"+n+":~s~ \"Points for style! Still not a hooker though.\"", "~r~"+n+":~s~ \"Love the compliment. Wrong girl.\"" };
                        case "Party Girl":   return new string[] { "~r~"+n+":~s~ \"Ha, thanks! Still not for sale though.\"", "~r~"+n+":~s~ \"Flattery's fun. Still no.\"", "~r~"+n+":~s~ \"Appreciate it. Not what I am.\"" };
                        case "Sarcastic":    return new string[] { "~r~"+n+":~s~ \"Flattery's nice. Still not a hooker. Move on.\"", "~r~"+n+":~s~ \"I appreciate it. I'm still not for hire though.\"" };
                        case "Cold":         return new string[] { "~r~"+n+":~s~ \"Thanks. Still not for sale.\"", "~r~"+n+":~s~ \"Flattery doesn't change what I am. Move on.\"", "~r~"+n+":~s~ \"I'm not for hire. That's final.\"" };
                        case "Independent":  return new string[] { "~r~"+n+":~s~ \"Thanks, but I'm not for sale.\"", "~r~"+n+":~s~ \"I appreciate the compliment. I don't do that though.\"", "~r~"+n+":~s~ \"You seem nice, but I'm not a hooker. Okay?\"" };
                        case "Classy":       return new string[] { "~r~"+n+":~s~ \"That's very kind. I'm still not for hire.\"", "~r~"+n+":~s~ \"I appreciate the grace. But no.\"", "~r~"+n+":~s~ \"Well said. Still not a sex worker.\"" };
                        case "Gold Digger":  return new string[] { "~r~"+n+":~s~ \"I appreciate it. I'm still not for hire though.\"", "~r~"+n+":~s~ \"Flattery's nice. Still not a hooker. Move on.\"", "~r~"+n+":~s~ \"Keep the charm. I'm still not working.\"" };
                        case "Manipulative": return new string[] { "~r~"+n+":~s~ \"Good line. I'm still not a hooker.\"", "~r~"+n+":~s~ \"That was well played. Still wrong call.\"", "~r~"+n+":~s~ \"Appreciate the effort. Not what I am.\"" };
                        case "Street Smart": return new string[] { "~r~"+n+":~s~ \"Nice line. Still the wrong girl.\"", "~r~"+n+":~s~ \"I hear you. Still not what I am.\"", "~r~"+n+":~s~ \"Appreciate it. Not a hooker.\"" };
                        case "Mysterious":   return new string[] { "~r~"+n+":~s~ \"Noted. Still no.\"", "~r~"+n+":~s~ \"That's kind. I'm not for sale.\"", "~r~"+n+":~s~ \"Appreciated. Not what I am.\"" };
                        case "Jealous":      return new string[] { "~r~"+n+":~s~ \"That was sweet... but I'm not working.\"", "~r~"+n+":~s~ \"I appreciate it. Don't get the wrong idea.\"", "~r~"+n+":~s~ \"Thanks. I'm still not a hooker.\"" };
                        case "Dominant":     return new string[] { "~r~"+n+":~s~ \"Nice approach. I'm still not for sale.\"", "~r~"+n+":~s~ \"I appreciate you being respectful. Still no.\"", "~r~"+n+":~s~ \"That was gracious. Still not working.\"" };
                        case "Aggressive":   return new string[] { "~r~"+n+":~s~ \"Nice. Still not happening.\"", "~r~"+n+":~s~ \"I don't care about the flattery. I don't work.\"", "~r~"+n+":~s~ \"Compliment noted. Still a no.\"" };
                        case "Chaotic":      return new string[] { "~r~"+n+":~s~ \"OKAY that was actually good! Still no though!\"", "~r~"+n+":~s~ \"Ha! Smooth! Wrong girl but smooth!\"", "~r~"+n+":~s~ \"10 out of 10 for the line. Still no.\"" };
                        case "Unstable":     return new string[] { "~r~"+n+":~s~ \"That was really nice... I'm still not a hooker.\"", "~r~"+n+":~s~ \"Please don't say things like that, I'm not working.\"", "~r~"+n+":~s~ \"That was sweet. I'm just not— no.\"" };
                        default:             return new string[] { "~r~"+n+":~s~ \"Thanks, but I'm not for sale.\"", "~r~"+n+":~s~ \"I appreciate the compliment. I don't do that though.\"", "~r~"+n+":~s~ \"You seem nice, but I'm not a hooker. Okay?\"" };
                    }
                default: // Playful approach
                    switch (personality)
                    {
                        case "Shy":          return new string[] { "~r~"+n+":~s~ \"I... I'm not working. Please just leave me alone.\"", "~r~"+n+":~s~ \"That's not... I'm not for sale.\"" };
                        case "Sweet":        return new string[] { "~r~"+n+":~s~ \"Ha, that's funny! But no, I'm not a hooker.\"", "~r~"+n+":~s~ \"You're cute but I'm really not for sale.\"" };
                        case "Romantic":     return new string[] { "~r~"+n+":~s~ \"I appreciate the humor, but I'm really not working.\"", "~r~"+n+":~s~ \"That was charming. I'm still not for hire.\"" };
                        case "Needy":        return new string[] { "~r~"+n+":~s~ \"I don't think that was very funny... I'm not a hooker.\"", "~r~"+n+":~s~ \"Please don't joke about that. I'm not working.\"" };
                        case "Flirty":       return new string[] { "~r~"+n+":~s~ \"Ha! Creative. Still not a hooker.\"", "~r~"+n+":~s~ \"Love the energy. Wrong girl though.\"", "~r~"+n+":~s~ \"Nice try! Not what I am.\"" };
                        case "Playful":      return new string[] { "~r~"+n+":~s~ \"Ha — no. Not that kind of free.\"", "~r~"+n+":~s~ \"Okay, that's bold. And wrong. Bye.\"" };
                        case "Party Girl":   return new string[] { "~r~"+n+":~s~ \"Ha — no. Not that kind of free.\"", "~r~"+n+":~s~ \"Okay, that's bold. And wrong. Bye.\"" };
                        case "Sarcastic":    return new string[] { "~r~"+n+":~s~ \"Ha. Very creative. Still no.\"", "~r~"+n+":~s~ \"Witty. Wrong. Bye.\"", "~r~"+n+":~s~ \"I'll give you points for originality. Still no.\"" };
                        case "Cold":         return new string[] { "~r~"+n+":~s~ \"I'm not for hire. Not even a little.\"", "~r~"+n+":~s~ \"Wrong girl. I really don't do that.\"", "~r~"+n+":~s~ \"That's not funny. Leave me alone.\"" };
                        case "Independent":  return new string[] { "~r~"+n+":~s~ \"I'm not for hire. Not even a little.\"", "~r~"+n+":~s~ \"Wrong girl. I really don't do that.\"", "~r~"+n+":~s~ \"That's not funny. Leave me alone.\"" };
                        case "Classy":       return new string[] { "~r~"+n+":~s~ \"That was rather bold. I'm still not for hire.\"", "~r~"+n+":~s~ \"Charming attempt. I don't work.\"", "~r~"+n+":~s~ \"That was fun. I'm still not a sex worker.\"" };
                        case "Gold Digger":  return new string[] { "~r~"+n+":~s~ \"Ha! Bold. Still not a hooker.\"", "~r~"+n+":~s~ \"Nice try. Wrong girl.\"", "~r~"+n+":~s~ \"Cute approach. Still no.\"" };
                        case "Manipulative": return new string[] { "~r~"+n+":~s~ \"Ha. Clever. Still not working.\"", "~r~"+n+":~s~ \"That was fun. Completely wrong read.\"", "~r~"+n+":~s~ \"Nice try. I'm not for sale.\"" };
                        case "Street Smart": return new string[] { "~r~"+n+":~s~ \"Ha, points for creativity. Still no.\"", "~r~"+n+":~s~ \"Wrong girl but good try.\"", "~r~"+n+":~s~ \"Nice one. Still not a hooker.\"" };
                        case "Mysterious":   return new string[] { "~r~"+n+":~s~ \"Interesting. Wrong.\"", "~r~"+n+":~s~ \"Ha. No.\"", "~r~"+n+":~s~ \"Creative. Incorrect.\"" };
                        case "Jealous":      return new string[] { "~r~"+n+":~s~ \"Ha — that's bold. I'm still not a hooker.\"", "~r~"+n+":~s~ \"Did that actually work on someone? Not me.\"", "~r~"+n+":~s~ \"That was... something. Still no.\"" };
                        case "Dominant":     return new string[] { "~r~"+n+":~s~ \"Ha. Bold. Still not for sale.\"", "~r~"+n+":~s~ \"Nice try. Still no.\"", "~r~"+n+":~s~ \"That was creative. Wrong call.\"" };
                        case "Aggressive":   return new string[] { "~r~"+n+":~s~ \"That's not funny. Get lost.\"", "~r~"+n+":~s~ \"Wrong girl. Back off.\"", "~r~"+n+":~s~ \"Ha. No. Leave.\"" };
                        case "Chaotic":      return new string[] { "~r~"+n+":~s~ \"Ha — no. Not that kind of free.\"", "~r~"+n+":~s~ \"Okay, that's bold. And wrong. Bye.\"" };
                        case "Unstable":     return new string[] { "~r~"+n+":~s~ \"I'm not for hire. Not even a little.\"", "~r~"+n+":~s~ \"Wrong girl. I really don't do that.\"", "~r~"+n+":~s~ \"That's not funny. Leave me alone.\"" };
                        default:             return new string[] { "~r~"+n+":~s~ \"I'm not for hire. Not even a little.\"", "~r~"+n+":~s~ \"Wrong girl. I really don't do that.\"", "~r~"+n+":~s~ \"That's not funny. Leave me alone.\"" };
                    }
            }
        }

        /// <summary>
        /// Hostile accept responses for rep -1 encounters. She'll work but she's not happy about it.
        /// Tone is driven by the player's approach cluster, her personality, and warm/cold baseline.
        /// </summary>
        private static string[] GetProstHostileAcceptResponse(int cluster, bool warm, string p = "")
        {
            switch (cluster)
            {
                case 0: // Gentle approach — softness doesn't disarm her
                    switch (p)
                    {
                        case "Shy":          return new string[] { "Save it. I know what this is. What do you need.", "That doesn't work on me anymore. What?" };
                        case "Sweet":        return new string[] { "Don't be sweet with me. What do you want.", "That nice act won't work. What do you need.", "Save it. What?" };
                        case "Romantic":     return new string[] { "Don't try that with me. What do you want.", "That approach won't work. What.", "Save it. What do you need." };
                        case "Needy":        return new string[] { "Save it. I know what this is. What do you need.", "Don't try to get sympathy from me. What?" };
                        case "Flirty":       return new string[] { "Don't bother flirting. What do you want.", "That doesn't work on me anymore. What.", "Save the charm. What do you need." };
                        case "Playful":      return new string[] { "Don't try that approach with me. What do you want.", "Fine. Make it quick. What?", "I'm not charmed. What do you need." };
                        case "Party Girl":   return new string[] { "Save it. What do you want.", "Don't try to soften me up. What.", "Fine. Make it quick. What?" };
                        case "Sarcastic":    return new string[] { "Oh, very sweet. What do you want.", "Nice try. What is it.", "Cute. What do you need." };
                        case "Cold":         return new string[] { "Don't waste my time. What.", "I know who you are. Come on.", "Fine. What?" };
                        case "Independent":  return new string[] { "That approach doesn't work on me. What do you want.", "Save it. What.", "Don't try that. What do you need." };
                        case "Classy":       return new string[] { "Don't bother being polite. What do you want.", "I know this routine. What.", "Save the manners. What do you need." };
                        case "Gold Digger":  return new string[] { "Soft approach, noted. Still need the money first. What.", "Don't play nice. What do you want.", "What do you need. And have the cash." };
                        case "Manipulative": return new string[] { "I see through that. What do you want.", "Don't try to soften me up. What.", "That approach doesn't work on me. What do you need." };
                        case "Street Smart": return new string[] { "I see through that. What do you want.", "Don't play soft with me. What.", "I know this game. What do you need." };
                        case "Mysterious":   return new string[] { "Save it. What do you want.", "That doesn't work on me. What.", "Fine. What?" };
                        case "Jealous":      return new string[] { "Don't try to be gentle with me. What do you want.", "Save it. What do you need.", "That approach won't work. What?" };
                        case "Dominant":     return new string[] { "Save the gentle act. What do you want.", "Don't. What do you need.", "That approach doesn't work on me. What?" };
                        case "Aggressive":   return new string[] { "Save the gentle act. What do you want.", "Don't. What do you need.", "That approach doesn't work on me. What?" };
                        case "Chaotic":      return new string[] { "Don't try that with me. What.", "Save it. What do you want.", "That approach is boring. What do you need." };
                        case "Unstable":     return new string[] { "Don't be gentle with me right now. What do you want.", "Save it. What.", "Fine. What do you need." };
                        default:             return warm
                            ? new string[] { "Save it. I know what this is. What do you need.", "That doesn't work on me anymore. What?" }
                            : new string[] { "Don't try that approach with me. What do you want.", "Fine. Make it quick. What?" };
                    }

                case 1: // Confident approach — she's not impressed
                    switch (p)
                    {
                        case "Shy":          return new string[] { "You've got some nerve. But money's money. What do you need.", "I remember you. Don't push it. What?" };
                        case "Sweet":        return new string[] { "Don't act like we're good. What do you want.", "You've got nerve showing up like this. What.", "Money's money. What do you need." };
                        case "Romantic":     return new string[] { "Don't act like you're welcome here. What do you want.", "Confidence noted. What do you need.", "Don't push it. What?" };
                        case "Needy":        return new string[] { "You've got some nerve. But money's money. What do you need.", "I remember you. Don't push it. What?" };
                        case "Flirty":       return new string[] { "Don't flatter yourself. What do you want.", "I'm not impressed. What do you need.", "That confidence means nothing to me. What?" };
                        case "Playful":      return new string[] { "Don't act like we're good. What do you want.", "Your confidence doesn't impress me. What.", "What do you need." };
                        case "Party Girl":   return new string[] { "Don't act like we're good. What.", "Confidence noted. I still don't like you. What do you want.", "What do you need. Make it quick." };
                        case "Sarcastic":    return new string[] { "Bold of you. What do you need.", "Sure, confident. Whatever. What.", "I've seen better. What do you want." };
                        case "Cold":         return new string[] { "You're on thin ice. What.", "Don't act like we're good. What do you want.", "Spare me. What?" };
                        case "Independent":  return new string[] { "Don't flatter yourself. What do you want.", "Don't act like we're good. What do you want.", "I decide who gets my time. What?" };
                        case "Classy":       return new string[] { "Confidence doesn't impress me. What do you want.", "Don't act like you're entitled here. What.", "What do you need. And don't dawdle." };
                        case "Gold Digger":  return new string[] { "Money talks. You remember that. What do you want.", "I don't trust you, but I'll take your money. What.", "Don't flatter yourself. What?" };
                        case "Manipulative": return new string[] { "I know what you're doing. What do you want.", "Don't try to play me. What do you need.", "Confidence won't get you a discount. What?" };
                        case "Street Smart": return new string[] { "Don't act like you own the situation. What.", "I know what this is. What do you want.", "Keep it moving. What do you need." };
                        case "Mysterious":   return new string[] { "Don't act like we're good. What.", "Confidence noted. What do you want.", "What do you need." };
                        case "Jealous":      return new string[] { "You've got nerve. What do you want.", "Don't act like I'm happy to see you. What.", "What do you need. And don't linger." };
                        case "Dominant":     return new string[] { "I'm the one who decides the terms. What do you want.", "You've got nerve. What do you need.", "Don't act like you're in charge here. What?" };
                        case "Aggressive":   return new string[] { "Don't push me. What do you want.", "Confidence means nothing here. What.", "What do you need. Fast." };
                        case "Chaotic":      return new string[] { "Bold. Annoying. What do you want.", "Don't act like that impresses me. What.", "What. Now." };
                        case "Unstable":     return new string[] { "Don't act like we're fine. We're not. What.", "Your confidence doesn't help you here. What do you want.", "What do you need." };
                        default:             return warm
                            ? new string[] { "You've got some nerve. But money's money. What do you need.", "I remember you. Don't push it. What?" }
                            : new string[] { "Don't flatter yourself. What do you want.", "Don't act like we're good. What do you want." };
                    }

                case 2: // Cool approach — she matches it but stays hostile
                    switch (p)
                    {
                        case "Shy":          return new string[] { "Still showing up. Fine. What do you want.", "You again. Keep it moving. What." };
                        case "Sweet":        return new string[] { "Don't try to be smooth with me. What do you want.", "You again. What do you need.", "Keep it moving. What?" };
                        case "Romantic":     return new string[] { "Don't play it cool with me. What do you want.", "Still here. Fine. What.", "What do you need." };
                        case "Needy":        return new string[] { "Still showing up. Fine. What do you want.", "You again. Keep it moving. What." };
                        case "Flirty":       return new string[] { "Save the cool act. What do you want.", "Don't try to read me. What.", "What do you need." };
                        case "Playful":      return new string[] { "Short and quick. What do you want.", "I don't like you. But I'll work. What?", "What do you need." };
                        case "Party Girl":   return new string[] { "Short and quick. What do you want.", "You again. What do you need.", "What. Make it fast." };
                        case "Sarcastic":    return new string[] { "Very smooth. What do you need.", "Cool. Sure. What.", "I get it. What do you want." };
                        case "Cold":         return new string[] { "You again. Don't linger. What.", "Short and quick. What do you want.", "What. And make it fast." };
                        case "Independent":  return new string[] { "Don't try to match me. What do you want.", "Short and quick. What.", "I don't like you. What do you need." };
                        case "Classy":       return new string[] { "Don't try to play it cool. What do you want.", "I see what you're doing. What.", "What do you need." };
                        case "Gold Digger":  return new string[] { "Cool approach. Still need cash upfront. What.", "Don't waste my time. What do you want.", "What do you need. And have the money." };
                        case "Manipulative": return new string[] { "Don't try to read me. What do you want.", "I keep it cooler. What.", "You can't play me. What do you need." };
                        case "Street Smart": return new string[] { "Stay cool all you want. I know what this is. What.", "Don't try to read me. What do you want.", "I keep it cooler. What do you need." };
                        case "Mysterious":   return new string[] { "You again. What.", "Short and quick. What do you want.", "What do you need." };
                        case "Jealous":      return new string[] { "You again. Don't linger. What.", "Short and quick. What do you want.", "What do you need. And make it fast." };
                        case "Dominant":     return new string[] { "Don't try to match me. What do you want.", "I set the pace here. What.", "What do you need." };
                        case "Aggressive":   return new string[] { "Don't play games with me. What do you want.", "Keep it moving. What.", "What. Now." };
                        case "Chaotic":      return new string[] { "Don't try to be cool with me. What.", "What do you want. No games.", "What do you need. Fast." };
                        case "Unstable":     return new string[] { "You again. Don't push it. What.", "Short and quick. What do you want.", "What do you need." };
                        default:             return warm
                            ? new string[] { "Still showing up. Fine. What do you want.", "You again. Keep it moving. What." }
                            : new string[] { "Short and quick. What do you want.", "I don't like you. But I'll work. What?" };
                    }

                case 3: // Flattering approach — she's not buying it
                    switch (p)
                    {
                        case "Shy":          return new string[] { "Flattery won't change anything between us. What do you need.", "Don't bother with that. What do you want." };
                        case "Sweet":        return new string[] { "Don't bother being nice. What do you want.", "Flattery doesn't help you here. What.", "What do you need." };
                        case "Romantic":     return new string[] { "Don't try to charm me. What do you want.", "That doesn't work anymore. What.", "What do you need." };
                        case "Needy":        return new string[] { "Flattery won't change anything between us. What do you need.", "Don't bother with that. What do you want." };
                        case "Flirty":       return new string[] { "Save the flattery. What do you want.", "That approach won't work. What.", "What do you need." };
                        case "Playful":      return new string[] { "I know what you're doing. It stopped working. What?", "Not interested in the charm. What do you need.", "What do you want." };
                        case "Party Girl":   return new string[] { "Save it. What do you want.", "Flattery noted. Still don't like you. What.", "What do you need." };
                        case "Sarcastic":    return new string[] { "Oh, very smooth. Still hate you. What.", "Flattery. Noted. What do you want.", "Ha. What do you need." };
                        case "Cold":         return new string[] { "That doesn't work on me. What do you want.", "Save it. What.", "Not interested in the charm. What do you need." };
                        case "Independent":  return new string[] { "Flattery doesn't get you anything here. What.", "I know what you're doing. What do you want.", "What do you need." };
                        case "Classy":       return new string[] { "Flattery is transparent. What do you want.", "Save the pleasantries. What.", "What do you need." };
                        case "Gold Digger":  return new string[] { "Flattery won't raise your discount. What do you want.", "Save the sweet words. What do you need.", "That doesn't earn you anything. What?" };
                        case "Manipulative": return new string[] { "Don't try to flatter me. I know this game. What.", "Save it. What do you want.", "What do you need. I'm not buying it." };
                        case "Street Smart": return new string[] { "Don't try to charm me. What do you want.", "Flattery is wasted on me. What.", "What do you need. Cut to it." };
                        case "Mysterious":   return new string[] { "Save it. What do you want.", "That doesn't work. What.", "What do you need." };
                        case "Jealous":      return new string[] { "Don't try to charm me. I don't trust you. What.", "Flattery won't help. What do you want.", "What do you need." };
                        case "Dominant":     return new string[] { "That doesn't work on me. Not from you. What.", "Compliments don't change anything. What do you want.", "Save it. What?" };
                        case "Aggressive":   return new string[] { "Save it. I'm not impressed. What do you want.", "Flattery means nothing. What.", "What do you need." };
                        case "Chaotic":      return new string[] { "Ha. Flattery. Whatever. What do you want.", "Save the sweet talk. What.", "What do you need. Now." };
                        case "Unstable":     return new string[] { "Don't try to charm me right now. What do you want.", "Save it. What.", "What do you need." };
                        default:             return warm
                            ? new string[] { "Flattery won't change anything between us. What do you need.", "Don't bother with that. What do you want." }
                            : new string[] { "I know what you're doing. It stopped working. What?", "Not interested in the charm. What do you need." };
                    }

                case 4: // Playful approach — she's not amused
                    switch (p)
                    {
                        case "Shy":          return new string[] { "I'm not laughing. What do you want.", "Yeah, very funny. What do you need." };
                        case "Sweet":        return new string[] { "I'm not in the mood for jokes. What do you want.", "Not funny. What do you need.", "What do you want." };
                        case "Romantic":     return new string[] { "Don't joke around with me. What do you want.", "Not in the mood. What.", "What do you need." };
                        case "Needy":        return new string[] { "I'm not laughing. What do you want.", "Yeah, very funny. What do you need." };
                        case "Flirty":       return new string[] { "Save the jokes. What do you want.", "Not funny. What.", "What do you need." };
                        case "Playful":      return new string[] { "Yeah, hilarious. What do you want.", "Not in the mood. What do you want.", "What do you need." };
                        case "Party Girl":   return new string[] { "Not in the mood. What do you want.", "Yeah, great. What do you need.", "What. Now." };
                        case "Sarcastic":    return new string[] { "Ha. Very droll. What do you need.", "Hilarious. What do you want.", "Sure, very funny. What?" };
                        case "Cold":         return new string[] { "Cut it. What do you want.", "This stopped being funny. What.", "Not in the mood. What do you want." };
                        case "Independent":  return new string[] { "Not funny. What do you want.", "Cut the act. What.", "What do you need." };
                        case "Classy":       return new string[] { "I'm not amused. What do you want.", "Don't waste my time. What.", "What do you need." };
                        case "Gold Digger":  return new string[] { "Jokes won't get you a discount. What do you want.", "Not funny. What do you need.", "What. And have the money." };
                        case "Manipulative": return new string[] { "Don't try to be cute. What do you want.", "I'm not playing. What.", "What do you need." };
                        case "Street Smart": return new string[] { "Don't play games with me. What do you want.", "Cut the act. What.", "What do you need." };
                        case "Mysterious":   return new string[] { "Not funny. What do you want.", "Cut it. What.", "What do you need." };
                        case "Jealous":      return new string[] { "Not in the mood for jokes. What do you want.", "Don't mess around. What.", "What do you need." };
                        case "Dominant":     return new string[] { "Don't play games with me. What do you want.", "I'm not laughing. What do you need.", "You want to mess around? What." };
                        case "Aggressive":   return new string[] { "Don't joke around with me. What do you want.", "Not funny. What.", "You think this is funny? What do you need." };
                        case "Chaotic":      return new string[] { "HA. Still not funny to me. What do you want.", "What? What do you need.", "Not in the mood. What." };
                        case "Unstable":     return new string[] { "Don't joke with me right now. What do you want.", "Not funny. What do you need.", "What." };
                        default:             return warm
                            ? new string[] { "I'm not laughing. What do you want.", "Yeah, very funny. What do you need." }
                            : new string[] { "Yeah, hilarious. What do you want.", "Not in the mood. What do you want." };
                    }

                default:
                    return new string[] { "I don't like you. But I'll take your money. What do you want." };
            }
        }

        /// <summary>
        /// Sandbox A-Life accept lines — per-personality responses when a casual NPC accepts the approach.
        /// </summary>
        private static string[] GetSandboxAcceptLines(string name, string p)
        {
            switch (p)
            {
                case "Flirty":       return new string[] { "~g~"+name+":~s~ \"Took you long enough to ask~s~.\"", "~g~"+name+":~s~ \"Oh, I was hoping you'd say that.\"", "~g~"+name+":~s~ \"Mmm. Yes. Come on.\"" };
                case "Dominant":     return new string[] { "~g~"+name+":~s~ \"Good. Keep up.\"", "~g~"+name+":~s~ \"Sure. But we do this on my terms.\"", "~g~"+name+":~s~ \"Fine. Don't slow me down.\"" };
                case "Shy":          return new string[] { "~g~"+name+":~s~ \"Oh... um. Okay. Sure.\"", "~g~"+name+":~s~ \"I mean... yeah. I guess.\"", "~g~"+name+":~s~ \"Okay... just don't be weird about it.\"" };
                case "Sweet":        return new string[] { "~g~"+name+":~s~ \"Aw, of course! I'd love that.\"", "~g~"+name+":~s~ \"Yeah! That sounds really nice.\"", "~g~"+name+":~s~ \"Oh yay. Let's go!\"" };
                case "Cold":         return new string[] { "~g~"+name+":~s~ \"Fine.\"", "~g~"+name+":~s~ \"Sure. Whatever.\"", "~g~"+name+":~s~ \"Alright. Let's make it quick.\"" };
                case "Sarcastic":    return new string[] { "~g~"+name+":~s~ \"Oh sure, why not. My schedule's wide open.\"", "~g~"+name+":~s~ \"Yeah, okay. This'll be fun. Probably.\"", "~g~"+name+":~s~ \"Fine. But I'm judging every second of this.\"" };
                case "Gold Digger":  return new string[] { "~g~"+name+":~s~ \"Hmm. Okay. But you're buying me something nice later.\"", "~g~"+name+":~s~ \"Sure. You better make it worth my while.\"", "~g~"+name+":~s~ \"Alright. Don't be cheap about it.\"" };
                case "Street Smart": return new string[] { "~g~"+name+":~s~ \"Yeah, okay. I got time.\"", "~g~"+name+":~s~ \"Sure. Let's go.\"", "~g~"+name+":~s~ \"Alright. Keep it clean.\"" };
                case "Party Girl":   return new string[] { "~g~"+name+":~s~ \"Yes! Let's go, let's go!\"", "~g~"+name+":~s~ \"Oh hell yeah. I'm in.\"", "~g~"+name+":~s~ \"Finally some fun. Let's do it.\"" };
                case "Romantic":     return new string[] { "~g~"+name+":~s~ \"I was hoping you'd ask~s~.\"", "~g~"+name+":~s~ \"Yes. I'd really like that.\"", "~g~"+name+":~s~ \"Of course. Come on.\"" };
                case "Needy":        return new string[] { "~g~"+name+":~s~ \"Yes! Oh, yes. Finally.\"", "~g~"+name+":~s~ \"I was starting to think you'd never ask.\"", "~g~"+name+":~s~ \"Yes. Please don't change your mind.\"" };
                case "Independent":  return new string[] { "~g~"+name+":~s~ \"Sure. But I do things my way.\"", "~g~"+name+":~s~ \"Okay. Just don't hover.\"", "~g~"+name+":~s~ \"Yeah. Fine. Let's go.\"" };
                case "Jealous":      return new string[] { "~g~"+name+":~s~ \"Okay. But you better not be seeing anyone else.\"", "~g~"+name+":~s~ \"Sure. Just me though, right?\"", "~g~"+name+":~s~ \"Alright. And I mean it — just us.\"" };
                case "Chaotic":      return new string[] { "~g~"+name+":~s~ \"Sure! Wait — what are we doing again? Doesn't matter, yes!\"", "~g~"+name+":~s~ \"Oh, that sounds crazy. I'm in.\"", "~g~"+name+":~s~ \"Yeah! Honestly I wasn't doing anything anyway.\"" };
                case "Manipulative": return new string[] { "~g~"+name+":~s~ \"I thought you'd never ask. I've been waiting.\"", "~g~"+name+":~s~ \"Of course. I just knew you'd come around.\"", "~g~"+name+":~s~ \"Sure. You won't regret choosing me.\"" };
                case "Aggressive":   return new string[] { "~g~"+name+":~s~ \"Fine. But don't waste my time.\"", "~g~"+name+":~s~ \"Yeah, alright. Keep it moving.\"", "~g~"+name+":~s~ \"Okay. But I'm not babysitting you.\"" };
                case "Playful":      return new string[] { "~g~"+name+":~s~ \"Oooh! Yes! This is gonna be fun.\"", "~g~"+name+":~s~ \"Ha! Sure, I'm game.\"", "~g~"+name+":~s~ \"Yeah! Let's see what happens.\"" };
                case "Mysterious":   return new string[] { "~g~"+name+":~s~ \"...Alright. Let's go.\"", "~g~"+name+":~s~ \"Sure. I had a feeling you'd ask.\"", "~g~"+name+":~s~ \"Fine. Don't read too much into it.\"" };
                case "Classy":       return new string[] { "~g~"+name+":~s~ \"Alright. But let's keep it tasteful.\"", "~g~"+name+":~s~ \"Sure. I appreciate you asking properly.\"", "~g~"+name+":~s~ \"Yes. That works for me.\"" };
                case "Unstable":     return new string[] { "~g~"+name+":~s~ \"Yes! No — wait — yes. Yes. Let's go.\"", "~g~"+name+":~s~ \"Okay okay okay. Fine. Yeah.\"", "~g~"+name+":~s~ \"I'm saying yes before I change my mind. Go.\"" };
                default:             return new string[] { "~g~"+name+":~s~ \"Sure, why not?\"", "~g~"+name+":~s~ \"Okay. Let's go.\"", "~g~"+name+":~s~ \"Alright, I'm in.\"" };
            }
        }

        /// <summary>Per-personality "leave me alone" lines shown when reputation drops to Avoiding (-1).</summary>
        private static string[] GetAvoidingReactionLines(string name, string p)
        {
            switch (p)
            {
                case "Flirty":       return new string[] { "~r~"+name+":~s~ \"You're cute, but no. Back off.\"", "~r~"+name+":~s~ \"Not happening anymore. We're done here.\"" };
                case "Dominant":     return new string[] { "~r~"+name+":~s~ \"I said enough. Walk away.\"", "~r~"+name+":~s~ \"We're done. Don't make me say it twice.\"" };
                case "Shy":          return new string[] { "~r~"+name+":~s~ \"Please... just leave me alone. Please.\"", "~r~"+name+":~s~ \"I really don't want to talk anymore. I'm going.\"" };
                case "Sweet":        return new string[] { "~r~"+name+":~s~ \"Hey, I said no. Please leave me alone, okay?\"", "~r~"+name+":~s~ \"I'm really not comfortable. I have to go now.\"" };
                case "Cold":         return new string[] { "~r~"+name+":~s~ \"No.\"", "~r~"+name+":~s~ \"We're done here. Don't follow me.\"" };
                case "Sarcastic":    return new string[] { "~r~"+name+":~s~ \"Oh wow, still here? No. Bye.\"", "~r~"+name+":~s~ \"Let me spell it out: N-O. Done.\"" };
                case "Gold Digger":  return new string[] { "~r~"+name+":~s~ \"You've wasted enough of my time. Goodbye.\"", "~r~"+name+":~s~ \"Not worth it. I'm leaving.\"" };
                case "Street Smart": return new string[] { "~r~"+name+":~s~ \"Take a hint. I'm done.\"", "~r~"+name+":~s~ \"Dead serious — leave me alone.\"" };
                case "Party Girl":   return new string[] { "~r~"+name+":~s~ \"Okay this stopped being fun. I'm out.\"", "~r~"+name+":~s~ \"Nooo. I'm done. Bye!\"" };
                case "Romantic":     return new string[] { "~r~"+name+":~s~ \"This isn't going anywhere. Please stop.\"", "~r~"+name+":~s~ \"I'm asking you nicely — leave me alone.\"" };
                case "Needy":        return new string[] { "~r~"+name+":~s~ \"Why do you keep doing this? Just go!\"", "~r~"+name+":~s~ \"I can't take this anymore. Leave me alone.\"" };
                case "Independent":  return new string[] { "~r~"+name+":~s~ \"I don't need this. I'm gone.\"", "~r~"+name+":~s~ \"Back off. I can handle myself.\"" };
                case "Jealous":      return new string[] { "~r~"+name+":~s~ \"You've been messing with too many people. We're done.\"", "~r~"+name+":~s~ \"I don't want to see you near me again.\"" };
                case "Chaotic":      return new string[] { "~r~"+name+":~s~ \"Nope nope nope — I'm leaving, bye, goodbye!\"", "~r~"+name+":~s~ \"I can't with this right now. I'm out!\"" };
                case "Manipulative": return new string[] { "~r~"+name+":~s~ \"You've had your chances. I'm done with you.\"", "~r~"+name+":~s~ \"This is over. Don't try to change my mind.\"" };
                case "Aggressive":   return new string[] { "~r~"+name+":~s~ \"Back off. I'm serious.\"", "~r~"+name+":~s~ \"Come near me again and I'm calling the cops.\"" };
                case "Playful":      return new string[] { "~r~"+name+":~s~ \"Ha — no. Game over. I'm leaving.\"", "~r~"+name+":~s~ \"Okay we're done playing. Get lost.\"" };
                case "Mysterious":   return new string[] { "~r~"+name+":~s~ \"...Leave me alone.\"", "~r~"+name+":~s~ \"I'm done. Don't follow me.\"" };
                case "Classy":       return new string[] { "~r~"+name+":~s~ \"I'd appreciate it if you kept your distance. Thank you.\"", "~r~"+name+":~s~ \"Please don't approach me again. We're done.\"" };
                case "Unstable":     return new string[] { "~r~"+name+":~s~ \"I swear to God, leave me alone!\"", "~r~"+name+":~s~ \"I can't — just go. GO!\"" };
                default:             return new string[] { "~r~"+name+":~s~ \"Leave me alone.\"", "~r~"+name+":~s~ \"Don't bother. I'm leaving.\"" };
            }
        }

        /// <summary>Relationship-tier goodbye lines for Prostitution A-Life, varied by leave cluster.</summary>
        private static string[] GetProstLeaveReactionLines(string name, string relationship, int leaveCluster)
        {
            switch (relationship)
            {
                case "Obsessed":
                    if (leaveCluster == 3) return new string[] { "~g~"+name+":~s~ \"Come find me whenever. I mean it.\"", "~g~"+name+":~s~ \"I'll be thinking about you.\"", "~g~"+name+":~s~ \"Don't make me wait too long.\"" };
                    if (leaveCluster == 4) return new string[] { "~g~"+name+":~s~ \"Stop it. Come back soon.\"", "~g~"+name+":~s~ \"Ugh, fine. But you better be back.\"", "~g~"+name+":~s~ \"Go then. I want you back soon.\"" };
                    if (leaveCluster == 1) return new string[] { "~g~"+name+":~s~ \"You better come back.\"", "~g~"+name+":~s~ \"I'll hold you to that.\"", "~g~"+name+":~s~ \"Don't forget about me.\"" };
                    return new string[] { "~g~"+name+":~s~ \"Already?\"", "~g~"+name+":~s~ \"You just got here...\"", "~g~"+name+":~s~ \"Fine. Come back soon.\"" };
                case "Regular":
                    if (leaveCluster == 3) return new string[] { "~g~"+name+":~s~ \"I'll be here. Take care.\"", "~g~"+name+":~s~ \"Safe travels.\"", "~g~"+name+":~s~ \"Look after yourself.\"" };
                    if (leaveCluster == 4) return new string[] { "~g~"+name+":~s~ \"You're not so bad yourself.\"", "~g~"+name+":~s~ \"Ha. See you.\"", "~g~"+name+":~s~ \"Not bad. Come back sometime.\"" };
                    if (leaveCluster == 1) return new string[] { "~g~"+name+":~s~ \"You know where I am.\"", "~g~"+name+":~s~ \"I'll be around.\"", "~g~"+name+":~s~ \"Same spot.\"" };
                    if (leaveCluster == 2) return new string[] { "~g~"+name+":~s~ \"Appreciated.\"", "~g~"+name+":~s~ \"Good doing business.\"", "~g~"+name+":~s~ \"Thanks for stopping by.\"" };
                    return new string[] { "~g~"+name+":~s~ \"See you around.\"", "~g~"+name+":~s~ \"Take care.\"", "~g~"+name+":~s~ \"Later.\"" };
                case "Avoiding":
                    return new string[] { "~r~"+name+":~s~ \"Okay.\"", "~r~"+name+":~s~ \"Right.\"", "~r~"+name+":~s~ \"Sure.\"" };
                case "Hostile":
                    return new string[] { "~r~"+name+":~s~ \"Don't rush back.\"", "~r~"+name+":~s~ \"Good.\"", "~r~"+name+":~s~ \"Finally.\"" };
                default: // Stranger
                    if (leaveCluster == 3) return new string[] { "~g~"+name+":~s~ \"Stay safe out there.\"", "~g~"+name+":~s~ \"Take care of yourself.\"", "~g~"+name+":~s~ \"Be careful out there.\"" };
                    if (leaveCluster == 4) return new string[] { "~g~"+name+":~s~ \"Ha. Sure.\"", "~g~"+name+":~s~ \"Sure, yeah.\"", "~g~"+name+":~s~ \"Okay then.\"" };
                    return new string[] { "~g~"+name+":~s~ \"Mmhm.\"", "~g~"+name+":~s~ \"Alright.\"", "~g~"+name+":~s~ \"Sure.\"" };
            }
        }

        /// <summary>Girlfriend's \"I love you too\" responses when player says \"I love you\" on leave.</summary>
        private static string[] GetGfILYResponseLines(string name, string p)
        {
            switch (p)
            {
                case "Romantic":    return new string[] { "~g~"+name+":~s~ \"I love you too. So much.\"", "~g~"+name+":~s~ \"I love you more than you know.\"", "~g~"+name+":~s~ \"I love you. Every day.\"" };
                case "Needy":       return new string[] { "~g~"+name+":~s~ \"I love you. Please come back soon.\"", "~g~"+name+":~s~ \"I love you so much. Don't be gone long.\"", "~g~"+name+":~s~ \"I love you too. I'll miss you.\"" };
                case "Shy":         return new string[] { "~g~"+name+":~s~ \"I... I love you too.\"", "~g~"+name+":~s~ \"You know I do. I love you.\"", "~g~"+name+":~s~ \"I love you. I just... yeah. I do.\"" };
                case "Playful":     return new string[] { "~g~"+name+":~s~ \"Ugh, finally! I love you too!\"", "~g~"+name+":~s~ \"TOOK YOU LONG ENOUGH. Love you too!\"", "~g~"+name+":~s~ \"Ha! I love you too, obviously.\"" };
                case "Cold":        return new string[] { "~g~"+name+":~s~ \"...I know.\"", "~g~"+name+":~s~ \"I heard you.\"", "~g~"+name+":~s~ \"Don't make it weird.\"" };
                case "Sarcastic":   return new string[] { "~g~"+name+":~s~ \"About time you said it. I love you too.\"", "~g~"+name+":~s~ \"Wow, he can say it. I love you too.\"", "~g~"+name+":~s~ \"Finally. Yeah, I love you.\"" };
                case "Jealous":     return new string[] { "~g~"+name+":~s~ \"I love you. Don't forget that.\"", "~g~"+name+":~s~ \"I love you. And only you, okay?\"", "~g~"+name+":~s~ \"I love you too. Just... only me, got it?\"" };
                case "Dominant":    return new string[] { "~g~"+name+":~s~ \"You better mean that.\"", "~g~"+name+":~s~ \"I know. I love you too.\"", "~g~"+name+":~s~ \"Good. Because I love you too.\"" };
                case "Chaotic":     return new string[] { "~g~"+name+":~s~ \"SAME! Go, go, love you, bye!\"", "~g~"+name+":~s~ \"I love you! Okay bye now!\"", "~g~"+name+":~s~ \"YES! Love you! Go! Okay bye!\"" };
                default:            return new string[] { "~g~"+name+":~s~ \"I love you too.\"", "~g~"+name+":~s~ \"I love you too. Take care.\"", "~g~"+name+":~s~ \"Love you too.\"" };
            }
        }

        /// <summary>NPC rejection responses when a non-girlfriend hears \"I love you\" on leave.</summary>
        private static string[] GetILYRejectionLines(string name, string p)
        {
            switch (p)
            {
                case "Cold":        return new string[] { "~r~"+name+":~s~ \"Don't say that.\"", "~r~"+name+":~s~ \"No. Stop.\"", "~r~"+name+":~s~ \"That's not what this is.\"" };
                case "Sarcastic":   return new string[] { "~r~"+name+":~s~ \"Wow. No. Absolutely not.\"", "~r~"+name+":~s~ \"...Okay. No. Just no.\"", "~r~"+name+":~s~ \"That's way too much.\"" };
                case "Aggressive":  return new string[] { "~r~"+name+":~s~ \"What the hell? Back off.\"", "~r~"+name+":~s~ \"You don't even know me. Stop.\"", "~r~"+name+":~s~ \"Too far. Way too far.\"" };
                case "Independent": return new string[] { "~r~"+name+":~s~ \"I barely know you. Please stop.\"", "~r~"+name+":~s~ \"That's not something I can say back.\"", "~r~"+name+":~s~ \"I need you to pump the brakes.\"" };
                case "Dominant":    return new string[] { "~r~"+name+":~s~ \"That's way too much. Pump the brakes.\"", "~r~"+name+":~s~ \"We're not there. Not even close.\"", "~r~"+name+":~s~ \"Don't say that to me. Not yet.\"" };
                case "Shy":         return new string[] { "~r~"+name+":~s~ \"I— that's a lot. I'm not there.\"", "~r~"+name+":~s~ \"I can't... that's too big right now.\"", "~r~"+name+":~s~ \"That's... a lot. I'm not ready.\"" };
                case "Mysterious":  return new string[] { "~r~"+name+":~s~ \"That's not where we are.\"", "~r~"+name+":~s~ \"Don't go there.\"", "~r~"+name+":~s~ \"...No.\"" };
                case "Romantic":    return new string[] { "~r~"+name+":~s~ \"I want to feel that. I just... don't yet. I'm sorry.\"", "~r~"+name+":~s~ \"I wish I could say it back. I'm not there yet.\"", "~r~"+name+":~s~ \"That means everything. I just... need more time.\"" };
                case "Needy":       return new string[] { "~r~"+name+":~s~ \"That's... really sweet. I'm not sure I'm there yet.\"", "~r~"+name+":~s~ \"I— wow. I really like you. Just not... yet.\"", "~r~"+name+":~s~ \"Please don't be upset. I'm just not ready.\"" };
                default:            return new string[] { "~r~"+name+":~s~ \"That's too much. I'm not ready for that.\"", "~r~"+name+":~s~ \"Slow down. We're not there.\"", "~r~"+name+":~s~ \"I can't say that back. I'm sorry.\"" };
            }
        }

        /// <summary>Personality-matched goodbye lines, varied by whether the player's leave style matched her preference.</summary>
        private static string[] GetLeaveReactionLines(string name, string p, bool matched)
        {
            switch (p)
            {
                case "Shy":          return matched ? new string[] { "~g~"+name+":~s~ \"I'll see you soon?\"", "~g~"+name+":~s~ \"Come back when you can, okay?\"", "~g~"+name+":~s~ \"Take care of yourself.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Oh... okay. Bye.\"", "~r~"+name+":~s~ \"Alright. See you.\"", "~r~"+name+":~s~ \"Okay. Bye.\"" };
                case "Sweet":        return matched ? new string[] { "~g~"+name+":~s~ \"Aw, I hope I see you again!\"", "~g~"+name+":~s~ \"Come back soon, okay?\"", "~g~"+name+":~s~ \"It was so nice talking to you!\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Oh, okay. Bye!\"", "~r~"+name+":~s~ \"Take care!\"", "~r~"+name+":~s~ \"Alright, see you around.\"" };
                case "Romantic":     return matched ? new string[] { "~g~"+name+":~s~ \"This was wonderful. Come back soon.\"", "~g~"+name+":~s~ \"I'll be thinking about you.\"", "~g~"+name+":~s~ \"Until we meet again.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Already leaving?\"", "~r~"+name+":~s~ \"Oh. Okay then.\"", "~r~"+name+":~s~ \"Take care.\"" };
                case "Needy":        return matched ? new string[] { "~g~"+name+":~s~ \"Don't stay away too long.\"", "~g~"+name+":~s~ \"Text me when you get home?\"", "~g~"+name+":~s~ \"Come back soon, promise?\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Oh. Fine.\"", "~r~"+name+":~s~ \"You're leaving already?\"", "~r~"+name+":~s~ \"Okay. I guess.\"" };
                case "Dominant":     return matched ? new string[] { "~g~"+name+":~s~ \"You know where to find me.\"", "~g~"+name+":~s~ \"Come back when you're ready.\"", "~g~"+name+":~s~ \"I'll be here.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Mmhm. Later.\"", "~r~"+name+":~s~ \"Sure.\"", "~r~"+name+":~s~ \"Fine. Go.\"" };
                case "Aggressive":   return matched ? new string[] { "~g~"+name+":~s~ \"Yeah, see you around.\"", "~g~"+name+":~s~ \"Stay out of trouble.\"", "~g~"+name+":~s~ \"Don't take too long.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Go, then.\"", "~r~"+name+":~s~ \"Whatever.\"", "~r~"+name+":~s~ \"Bye.\"" };
                case "Street Smart": return matched ? new string[] { "~g~"+name+":~s~ \"Stay out of trouble.\"", "~g~"+name+":~s~ \"Watch your back out there.\"", "~g~"+name+":~s~ \"Keep it real.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Aight.\"", "~r~"+name+":~s~ \"Sure.\"", "~r~"+name+":~s~ \"Later.\"" };
                case "Independent":  return matched ? new string[] { "~g~"+name+":~s~ \"Take care.\"", "~g~"+name+":~s~ \"Look after yourself.\"", "~g~"+name+":~s~ \"Until next time.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Sure.\"", "~r~"+name+":~s~ \"Okay.\"", "~r~"+name+":~s~ \"Bye.\"" };
                case "Cold":         return matched ? new string[] { "~g~"+name+":~s~ \"Until next time.\"", "~g~"+name+":~s~ \"Later.\"", "~g~"+name+":~s~ \"See you.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Noted.\"", "~r~"+name+":~s~ \"Sure.\"", "~r~"+name+":~s~ \"Right.\"" };
                case "Sarcastic":    return matched ? new string[] { "~g~"+name+":~s~ \"Try not to think about me too much.\"", "~g~"+name+":~s~ \"Good luck not missing me.\"", "~g~"+name+":~s~ \"I'll be here when you inevitably come back.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Oh wow, don't make me cry.\"", "~r~"+name+":~s~ \"Devastating. Truly.\"", "~r~"+name+":~s~ \"I'll manage somehow.\"" };
                case "Mysterious":   return matched ? new string[] { "~g~"+name+":~s~ \"You'll find me if you want to.\"", "~g~"+name+":~s~ \"I'll be around.\"", "~g~"+name+":~s~ \"Until we cross paths again.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Sure.\"", "~r~"+name+":~s~ \"Okay.\"", "~r~"+name+":~s~ \"Right.\"" };
                case "Classy":       return matched ? new string[] { "~g~"+name+":~s~ \"It was a pleasure. Until next time.\"", "~g~"+name+":~s~ \"Do take care.\"", "~g~"+name+":~s~ \"I look forward to seeing you again.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Of course.\"", "~r~"+name+":~s~ \"Goodbye then.\"", "~r~"+name+":~s~ \"Very well.\"" };
                case "Gold Digger":  return matched ? new string[] { "~g~"+name+":~s~ \"Let me know if you need anything.\"", "~g~"+name+":~s~ \"I'll be here.\"", "~g~"+name+":~s~ \"Come back anytime.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Mmhm.\"", "~r~"+name+":~s~ \"Sure.\"", "~r~"+name+":~s~ \"Okay.\"" };
                case "Manipulative": return matched ? new string[] { "~g~"+name+":~s~ \"You're going to miss me.\"", "~g~"+name+":~s~ \"I knew you'd come around. See you soon.\"", "~g~"+name+":~s~ \"I'll be here. I always am.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Leaving already? That's a shame.\"", "~r~"+name+":~s~ \"I thought you'd stay longer.\"", "~r~"+name+":~s~ \"Your loss.\"" };
                case "Jealous":      return matched ? new string[] { "~g~"+name+":~s~ \"Come back soon, okay?\"", "~g~"+name+":~s~ \"Don't make me wait too long.\"", "~g~"+name+":~s~ \"I'll be thinking about you.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Fine. Go.\"", "~r~"+name+":~s~ \"Okay then.\"", "~r~"+name+":~s~ \"Sure.\"" };
                case "Unstable":     return matched ? new string[] { "~g~"+name+":~s~ \"Okay! Text me when you get home!\"", "~g~"+name+":~s~ \"Come back soon! Like really soon!\"", "~g~"+name+":~s~ \"Bye! Be safe! Come back!\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Oh. Okay then.\"", "~r~"+name+":~s~ \"Fine. Bye.\"", "~r~"+name+":~s~ \"Okay. I guess.\"" };
                case "Flirty":       return matched ? new string[] { "~g~"+name+":~s~ \"Sure you don't want to stay a little longer?\"", "~g~"+name+":~s~ \"Come find me again sometime.\"", "~g~"+name+":~s~ \"I'll be thinking about you.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Aw, already?\"", "~r~"+name+":~s~ \"Already leaving?\"", "~r~"+name+":~s~ \"Bye then.\"" };
                case "Party Girl":   return matched ? new string[] { "~g~"+name+":~s~ \"This was fun! Next time, yeah?\"", "~g~"+name+":~s~ \"Had a blast! Come find me again!\"", "~g~"+name+":~s~ \"Don't be a stranger!\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Ugh, fine.\"", "~r~"+name+":~s~ \"Whatever, bye.\"", "~r~"+name+":~s~ \"Okay then.\"" };
                case "Playful":      return matched ? new string[] { "~g~"+name+":~s~ \"See ya! Don't miss me too much!\"", "~g~"+name+":~s~ \"Come back and play later!\"", "~g~"+name+":~s~ \"Bye! This was fun!\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Alright, bye.\"", "~r~"+name+":~s~ \"Okay then.\"", "~r~"+name+":~s~ \"Sure.\"" };
                case "Chaotic":      return matched ? new string[] { "~g~"+name+":~s~ \"BYE! Come back when things get boring!\"", "~g~"+name+":~s~ \"GO GO GO! Come back later!\"", "~g~"+name+":~s~ \"Bye! This was chaos in the best way!\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Okay, sure, whatever!\"", "~r~"+name+":~s~ \"Fine! Bye!\"", "~r~"+name+":~s~ \"Okay!\"" };
                default:             return matched ? new string[] { "~g~"+name+":~s~ \"Take care of yourself.\"", "~g~"+name+":~s~ \"Until next time.\"", "~g~"+name+":~s~ \"See you around.\"" }
                                                    : new string[] { "~r~"+name+":~s~ \"Bye.\"", "~r~"+name+":~s~ \"Sure.\"", "~r~"+name+":~s~ \"Okay.\"" };
            }
        }

        /// <summary>Per-personality acceptance lines when player asks to finish inside (casual A-Life, non-hooker).</summary>
        private static string[] GetFinishInsideAcceptLines(string name, string p)
        {
            string pre = "~g~" + name + ":~s~ \"";
            switch (p)
            {
                case "Flirty":
                    return new[] { pre + "Mmm... yes. Don't stop.\"",
                                   pre + "Oh please, yes. Please!\"",
                                   pre + "Yes~s~... I've been hoping you'd ask.\"" };
                case "Dominant":
                    return new[] { pre + "Yes. Do it. Now.\"",
                                   pre + "...Okay. This once.\"",
                                   pre + "Fine. But you do it my way.\"" };
                case "Shy":
                    return new[] { pre + "Oh... okay. Yeah.\"",
                                   pre + "Mhm... okay.\"",
                                   pre + "I... yeah. Okay.\"" };
                case "Sweet":
                    return new[] { pre + "Yes... I want you to.\"",
                                   pre + "Mhm... just this once.\"",
                                   pre + "Okay~s~... yes. Please.\"" };
                case "Cold":
                    return new[] { pre + "...Make it quick.\"",
                                   pre + "Fine. But don't read into it.\"",
                                   pre + "Okay. Don't make it weird.\"" };
                case "Sarcastic":
                    return new[] { pre + "Sure. Living dangerously today, I guess.\"",
                                   pre + "Fine. Don't make a big deal of it.\"",
                                   pre + "Sure. Against my better judgment.\"" };
                case "Gold Digger":
                    return new[] { pre + "Okay. You better make it worth my while.\"",
                                   pre + "Alright. But you owe me something nice.\"",
                                   pre + "Fine~s~... but I expect something in return.\"" };
                case "Street Smart":
                    return new[] { pre + "Okay. Fine.\"",
                                   pre + "Alright. Go ahead.\"",
                                   pre + "Sure. Just this once.\"" };
                case "Party Girl":
                    return new[] { pre + "Ohhh yes. Go for it.\"",
                                   pre + "Hell yeah. Don't hold back.\"",
                                   pre + "Yes! Do it!\"" };
                case "Romantic":
                    return new[] { pre + "Yes... I want that.\"",
                                   pre + "Oh please, yes. Please!\"",
                                   pre + "Yes~s~... stay with me.\"" };
                case "Needy":
                    return new[] { pre + "Yes! Yes, please. Yes.\"",
                                   pre + "Oh please, yes. Please don't stop.\"",
                                   pre + "Yes — don't stop — please.\"" };
                case "Independent":
                    return new[] { pre + "...Okay. This time.\"",
                                   pre + "Fine. Just this once.\"",
                                   pre + "Alright. But this is my call.\"" };
                case "Jealous":
                    return new[] { pre + "Yes. Only with me, remember that.\"",
                                   pre + "Okay... yeah. Just us.\"",
                                   pre + "Yes. But only ever me.\"" };
                case "Chaotic":
                    return new[] { pre + "Yes — wait — YES. Go.\"",
                                   pre + "Okay okay okay. Fine. Yeah.\"",
                                   pre + "Yes! Go! Before I overthink it!\"" };
                case "Manipulative":
                    return new[] { pre + "I was wondering when you'd ask.\"",
                                   pre + "Of course. I've been waiting.\"",
                                   pre + "I knew you would. Go ahead.\"" };
                case "Aggressive":
                    return new[] { pre + "Yeah. Do it.\"",
                                   pre + "Fine. Make it count.\"",
                                   pre + "Yes. Don't hold back.\"" };
                case "Playful":
                    return new[] { pre + "Ha! Yes. Do it.\"",
                                   pre + "Ooh. Yes please.\"",
                                   pre + "Yes~s~. Obviously.\"" };
                case "Mysterious":
                    return new[] { pre + "...Yes.\"",
                                   pre + "Alright. Go ahead.\"",
                                   pre + "...I'll allow it.\"" };
                case "Classy":
                    return new[] { pre + "...Yes. Okay.\"",
                                   pre + "Alright. But be gentle.\"",
                                   pre + "Yes. Just be careful.\"" };
                case "Unstable":
                    return new[] { pre + "Yes — I mean — yes. Do it.\"",
                                   pre + "Okay okay. Yes. Before I change my mind.\"",
                                   pre + "Yes! Now! Go!\"" };
                default:
                    return new[] { pre + "...Okay. Fine.\"",
                                   pre + "Alright. Go ahead.\"",
                                   pre + "Sure. This once.\"" };
            }
        }

        /// <summary>Personality-driven price-quote lines for finish-inside requests (hooker/prost modes).</summary>
        private static string[] GetFinishInsidePriceLines(string name, string p, int price)
        {
            string v = "$" + price;
            string pre = "~g~" + name + ":~s~ \"";
            switch (p)
            {
                case "Flirty":
                    return new[] { pre + "Ooh~s~. Sure. That's " + v + " extra.\"",
                                   pre + v + " and you can cum inside~s~. Deal?\"",
                                   pre + "Sure~s~. Just " + v + " more.\"" };
                case "Dominant":
                    return new[] { pre + v + ". Pay it and we proceed.\"",
                                   pre + "That's " + v + ". Non-negotiable.\"",
                                   pre + v + " and you do what I say.\"" };
                case "Cold":
                    return new[] { pre + v + ". Take it or leave it.\"",
                                   pre + "Sure. " + v + ". Make it quick.\"",
                                   pre + v + " and you can. Don't read into it.\"" };
                case "Sweet":
                    return new[] { pre + "Sure! That's just " + v + " extra, okay?\"",
                                   pre + v + " and you can. Just... be gentle.\"",
                                   pre + "Okay! " + v + " more. Is that alright?\"" };
                case "Sarcastic":
                    return new[] { pre + "Sure. That's " + v + ". Surprised you even asked.\"",
                                   pre + v + " and you can cum inside. You're welcome.\"",
                                   pre + "Yep. " + v + ". Totally normal transaction.\"" };
                case "Gold Digger":
                    return new[] { pre + "That'll be " + v + ". Every cent.\"",
                                   pre + v + " — and that's me being generous.\"",
                                   pre + v + " upfront. Then we talk.\"" };
                case "Street Smart":
                    return new[] { pre + v + ". You know the rules.\"",
                                   pre + "Sure. " + v + " extra. Don't try to haggle.\"",
                                   pre + v + " and you can. Simple.\"" };
                case "Party Girl":
                    return new[] { pre + "Yeah! Just " + v + " more, babe.\"",
                                   pre + v + " and you can cum inside. Easy deal!\"",
                                   pre + "Sure! " + v + ". Let's keep this going.\"" };
                case "Romantic":
                    return new[] { pre + "...Okay. " + v + " extra. Just... make it mean something.\"",
                                   pre + "Sure. " + v + ". I don't usually do this.\"",
                                   pre + v + " and you can. Just stay a little after.\"" };
                case "Shy":
                    return new[] { pre + "Um... sure. That'll be " + v + " though.\"",
                                   pre + v + " and... yeah. Okay.\"",
                                   pre + "I mean... " + v + " extra. If that's okay.\"" };
                case "Needy":
                    return new[] { pre + "Yes! " + v + ", okay? Just say yes.\"",
                                   pre + v + " and you can. Please.\"",
                                   pre + "Sure! " + v + " more. You won't regret it.\"" };
                case "Independent":
                    return new[] { pre + v + ". My terms. Take it.\"",
                                   pre + "Sure. " + v + " extra. That's how this works.\"",
                                   pre + v + " and you can. No discussion.\"" };
                case "Jealous":
                    return new[] { pre + v + ". And you'd better not be doing this with anyone else.\"",
                                   pre + "Sure. That's " + v + ". Just me, got it?\"",
                                   pre + v + " and you can cum inside. Only with me.\"" };
                case "Chaotic":
                    return new[] { pre + "Yes! " + v + "! Wait — yes! Deal!\"",
                                   pre + v + " and you can cum inside — go!\"",
                                   pre + "Sure! " + v + "! Don't make me think about it!\"" };
                case "Manipulative":
                    return new[] { pre + "I knew you'd want this. " + v + " extra.\"",
                                   pre + v + ". I think that's more than fair, don't you?\"",
                                   pre + "Sure. " + v + ". Consider it a privilege.\"" };
                case "Aggressive":
                    return new[] { pre + v + ". Pay it. Now.\"",
                                   pre + "Sure. " + v + " extra. Don't make me repeat it.\"",
                                   pre + v + " and you can cum inside. We clear?\"" };
                case "Playful":
                    return new[] { pre + "Ha! Sure. " + v + " extra though~s~.\"",
                                   pre + v + " and you can cum inside. Fun, right?\"",
                                   pre + "Sure~s~. " + v + ". You're lucky I like you.\"" };
                case "Mysterious":
                    return new[] { pre + "...Sure. " + v + ".\"",
                                   pre + v + " and you can. Don't ask why I'm agreeing.\"",
                                   pre + "Fine. " + v + ". That's all I'll say.\"" };
                case "Classy":
                    return new[] { pre + "That will be " + v + " extra. Understood?\"",
                                   pre + v + ". I expect it upfront.\"",
                                   pre + "Sure. " + v + ". Let's keep this tasteful.\"" };
                case "Unstable":
                    return new[] { pre + "Yes — " + v + " — just yes — go!\"",
                                   pre + v + " and you can cum inside. Okay? Okay!\"",
                                   pre + "Fine! " + v + "! Before I change my mind!\"" };
                default:
                    return new[] { pre + "Sure. That's " + v + " extra.\"",
                                   pre + v + " and you can cum inside.\"",
                                   pre + "It'll cost you " + v + ". Deal?\"" };
            }
        }

        /// <summary>Per-personality positive lines shown when NPC has CimPreference=true and player finishes inside unasked.</summary>
        private static string[] GetCimPreferenceUnaskedLines(string name, string p)
        {
            string pre = "~g~" + name + ":~s~ \"";
            const string suf = "\"";
            switch (p)
            {
                case "Flirty":
                    return new[] { pre + "Mmm~s~... yes. I wanted that." + suf,
                                   pre + "Mmm~s~... don't apologize. I liked it." + suf,
                                   pre + "You could do that again sometime." + suf };
                case "Dominant":
                    return new[] { pre + "Good. That's how I like it." + suf,
                                   pre + "You didn't ask. But I'll let it go~s~... this time." + suf,
                                   pre + "Just be grateful I'm this lenient." + suf };
                case "Cold":
                    return new[] { pre + "...Don't make it weird. I don't mind." + suf,
                                   pre + "Fine. I didn't say stop." + suf,
                                   pre + "Don't read into it." + suf };
                case "Sweet":
                    return new[] { pre + "Oh~s~... that's okay. I kind of wanted that." + suf,
                                   pre + "You should have asked... but~s~... I'm glad you didn't pull out." + suf,
                                   pre + "Just... maybe ask next time. Okay?" + suf };
                case "Sarcastic":
                    return new[] { pre + "Didn't ask, huh? Lucky you I'm into it." + suf,
                                   pre + "Wow. Bold move. It worked out for you this time." + suf,
                                   pre + "I'll be merciful and not make this a whole thing." + suf };
                case "Gold Digger":
                    return new[] { pre + "You didn't even ask. You owe me something for that." + suf,
                                   pre + "No charge~s~... this once. Don't get used to it." + suf,
                                   pre + "I'm keeping track, just so you know." + suf };
                case "Street Smart":
                    return new[] { pre + "You're lucky I actually like that." + suf,
                                   pre + "Risky move. Paid off for you." + suf,
                                   pre + "Don't get too comfortable though." + suf };
                case "Party Girl":
                    return new[] { pre + "Ohhh~s~. Yeah. I'm not even mad." + suf,
                                   pre + "Ha! I actually love that you did that." + suf,
                                   pre + "You're my favorite right now." + suf };
                case "Romantic":
                    return new[] { pre + "You didn't ask~s~... but I wanted you to. It's okay." + suf,
                                   pre + "I wished you had asked~s~... but I'm not upset." + suf,
                                   pre + "Next time, ask. I'll say yes." + suf };
                case "Shy":
                    return new[] { pre + "Oh~s~... um~s~... I~s~... it's okay. I liked it." + suf,
                                   pre + "You should have asked... but~s~... I don't mind." + suf,
                                   pre + "I'm~s~... glad you didn't." + suf };
                case "Needy":
                    return new[] { pre + "Oh yes~s~. I've been waiting for you to do that." + suf,
                                   pre + "You have no idea how much I wanted that." + suf,
                                   pre + "Do it again sometime. Please." + suf };
                case "Independent":
                    return new[] { pre + "Didn't ask. But~s~... I'm not complaining." + suf,
                                   pre + "You got lucky. I happen to like that." + suf,
                                   pre + "Just so we're clear — I'm not upset." + suf };
                case "Jealous":
                    return new[] { pre + "You'd better not be doing that with anyone else." + suf,
                                   pre + "I liked it. But next time — ask me first." + suf,
                                   pre + "Only ever with me." + suf };
                case "Chaotic":
                    return new[] { pre + "OH~s~. Yes. That was — yes. More of that." + suf,
                                   pre + "Did you just — yes. Perfect. Loved it." + suf,
                                   pre + "I don't even know what I'm feeling right now but I love it." + suf };
                case "Manipulative":
                    return new[] { pre + "You didn't ask~s~... but I'll count that as a point in your favor." + suf,
                                   pre + "Bold move. Turns out I liked it. You're welcome." + suf,
                                   pre + "Just know I remember these things." + suf };
                case "Aggressive":
                    return new[] { pre + "Yeah. Good." + suf,
                                   pre + "That's what I wanted anyway." + suf,
                                   pre + "Glad we're on the same page." + suf };
                case "Playful":
                    return new[] { pre + "Heyyy~s~. I didn't say you could~s~... but I'm not complaining." + suf,
                                   pre + "Ha! I like you more now." + suf,
                                   pre + "You earned some serious bonus points." + suf };
                case "Mysterious":
                    return new[] { pre + "...I didn't stop you." + suf,
                                   pre + "You figured something out about me just now." + suf,
                                   pre + "I'll leave it at that." + suf };
                case "Classy":
                    return new[] { pre + "You should have asked. But~s~... I'll allow it this once." + suf,
                                   pre + "Unannounced. Fortunately for you, I don't mind." + suf,
                                   pre + "You're lucky it worked in your favor." + suf };
                case "Unstable":
                    return new[] { pre + "Oh — oh that was — yes. I love that. I hate that I love that." + suf,
                                   pre + "That was unexpected and also exactly what I wanted." + suf,
                                   pre + "I need a minute~s~. That was a lot." + suf };
                default:
                    return new[] { pre + "...I liked that." + suf,
                                   pre + "You should have asked~s~... but it's fine." + suf,
                                   pre + "No need to explain." + suf };
            }
        }

        /// <summary>Per-relationship/personality angry lines when player finishes inside without asking (prost/casual-NPC mode).</summary>
        private static string[] GetFinishForcedProstLines(string name, string rel, string p)
        {
            string pre = "~r~" + name + ":~s~ \"";
            const string suf = "\"";
            switch (rel)
            {
                case "Obsessed":
                    if (p == "Shy" || p == "Sweet" || p == "Romantic" || p == "依賴")
                        return new string[] {
                            pre + "You should have asked me... that's not okay." + suf,
                            pre + "I would have said yes. Why didn't you just ask?" + suf,
                            pre + "That really wasn't okay. You know I would have said yes." + suf,
                        };
                    if (p == "Dominant" || p == "Aggressive")
                        return new string[] {
                            pre + "I don't care how long we've been doing this. You ask." + suf,
                            pre + "Don't ever do that again. Ever." + suf,
                            pre + "You know better than that." + suf,
                        };
                    return new string[] {
                        pre + "You didn't have to do that without asking." + suf,
                        pre + "I would have said yes. Just ask next time." + suf,
                        pre + "That wasn't your call to make." + suf,
                    };
                case "Regular":
                    if (p == "Shy" || p == "Sweet" || p == "Romantic")
                        return new string[] {
                            pre + "You didn't ask. That's not okay." + suf,
                            pre + "I'm not okay with that. You should have asked." + suf,
                            pre + "Please ask next time. That was not okay." + suf,
                        };
                    if (p == "Dominant" || p == "Aggressive" || p == "Cold")
                        return new string[] {
                            pre + "You don't do that without asking. Ever." + suf,
                            pre + "That was not agreed. We're done today." + suf,
                            pre + "Don't do that again." + suf,
                        };
                    if (p == "Sarcastic" || p == "Street Smart")
                        return new string[] {
                            pre + "Really? After all this time you still don't ask?" + suf,
                            pre + "You know how this works. You ask first." + suf,
                            pre + "Come on. You know better." + suf,
                        };
                    if (p == "Playful" || p == "Party Girl")
                        return new string[] {
                            pre + "Whoa \u2014 you were supposed to ask!" + suf,
                            pre + "Hey, that's not how we do things." + suf,
                            pre + "Not cool. You know you need to ask." + suf,
                        };
                    return new string[] {
                        pre + "You didn't ask. That's not okay." + suf,
                        pre + "We've done this before. You know you need to ask." + suf,
                        pre + "That wasn't your call." + suf,
                    };
                case "Acquaintance":
                    if (p == "Shy" || p == "Sweet")
                        return new string[] {
                            pre + "That was not okay. You should have asked." + suf,
                            pre + "Please don't do that again." + suf,
                            pre + "I didn't agree to that." + suf,
                        };
                    if (p == "Dominant" || p == "Aggressive" || p == "Cold")
                        return new string[] {
                            pre + "You don't do that. You ask." + suf,
                            pre + "That's not how this works. We're done." + suf,
                            pre + "Do that again and I'm gone." + suf,
                        };
                    return new string[] {
                        pre + "You should have asked first." + suf,
                        pre + "That wasn't agreed. Don't do that." + suf,
                        pre + "I didn't say you could do that." + suf,
                    };
                default: // Stranger
                    if (p == "Dominant" || p == "Aggressive" || p == "Cold" || p == "Street Smart")
                        return new string[] {
                            pre + "What the hell is wrong with you?" + suf,
                            pre + "Are you out of your mind? Get away from me." + suf,
                            pre + "Don't you EVER do that again." + suf,
                        };
                    if (p == "Shy" || p == "Sweet" || p == "Romantic")
                        return new string[] {
                            pre + "Oh no \u2014 I didn't say you could do that!" + suf,
                            pre + "That was not okay. I don't even know you." + suf,
                            pre + "Please leave. That was not okay." + suf,
                        };
                    if (p == "Sarcastic" || p == "Chaotic" || p == "Unstable")
                        return new string[] {
                            pre + "Are you SERIOUS right now?" + suf,
                            pre + "What the hell? You don't do that!" + suf,
                            pre + "I can't believe you just did that." + suf,
                        };
                    return new string[] {
                        pre + "You should have asked. That's not okay." + suf,
                        pre + "What is wrong with you?" + suf,
                        pre + "Get out. We're done." + suf,
                    };
            }
        }

        /// <summary>Per-relationship/personality decline lines for finish-inside requests (A-Life casual).</summary>
        private static string[] GetFinishInsideDeclineLines(string name, string rel, string p)
        {
            string pre = "~r~" + name + ":~s~ \"";
            const string suf = "\"";
            switch (rel)
            {
                case "Obsessed":
                    return new string[] {
                        pre + "Pull out, baby. Not today." + suf,
                        pre + "Not this time, okay? I'm sorry." + suf,
                    };
                case "Regular":
                    if (p == "Shy" || p == "Sweet" || p == "Romantic")
                        return new string[] {
                            pre + "I'd rather you didn't... pull out, okay?" + suf,
                            pre + "Not inside. Sorry." + suf,
                            pre + "Please pull out." + suf,
                        };
                    if (p == "Dominant" || p == "Aggressive" || p == "Cold")
                        return new string[] {
                            pre + "No. Pull out." + suf,
                            pre + "Not happening. Out." + suf,
                            pre + "I said no. Do it." + suf,
                        };
                    if (p == "Sarcastic" || p == "Street Smart")
                        return new string[] {
                            pre + "Ha. No. Pull out." + suf,
                            pre + "Yeah, that's not part of the deal." + suf,
                            pre + "Nice try. Out." + suf,
                        };
                    return new string[] {
                        pre + "No, pull out." + suf,
                        pre + "Not inside. Come on." + suf,
                        pre + "That's not happening." + suf,
                    };
                case "Acquaintance":
                    if (p == "Shy" || p == "Sweet")
                        return new string[] {
                            pre + "Oh — no, please pull out." + suf,
                            pre + "No, not that. Pull out." + suf,
                            pre + "Please, pull out." + suf,
                        };
                    if (p == "Dominant" || p == "Aggressive")
                        return new string[] {
                            pre + "No. Pull out now." + suf,
                            pre + "Not inside. We haven't even talked about that." + suf,
                            pre + "I said no. Out." + suf,
                        };
                    return new string[] {
                        pre + "No. Pull out." + suf,
                        pre + "That wasn't agreed. Pull out." + suf,
                        pre + "No, not inside." + suf,
                    };
                default: // Stranger
                    if (p == "Shy" || p == "Sweet" || p == "Romantic")
                        return new string[] {
                            pre + "No — please pull out." + suf,
                            pre + "That's not okay. Pull out." + suf,
                            pre + "Please, just pull out." + suf,
                        };
                    if (p == "Dominant" || p == "Aggressive" || p == "Cold")
                        return new string[] {
                            pre + "No. Out. Now." + suf,
                            pre + "Don't. Pull out." + suf,
                            pre + "I don't know you. Pull out." + suf,
                        };
                    if (p == "Sarcastic")
                        return new string[] {
                            pre + "Absolutely not. Pull out." + suf,
                            pre + "Yeah, no. Out." + suf,
                            pre + "Not a chance. Pull out." + suf,
                        };
                    if (p == "Playful" || p == "Party Girl" || p == "Chaotic")
                        return new string[] {
                            pre + "Whoa, no. Pull out." + suf,
                            pre + "Ha — no. Out." + suf,
                            pre + "Not today. Pull out." + suf,
                        };
                    return new string[] {
                        pre + "No. Pull out." + suf,
                        pre + "That's not happening. Out." + suf,
                        pre + "Pull out." + suf,
                    };
            }
        }

        private string GetConvResponse(int branch, int item, ALifePedData d)
        {
            string p = (d != null && d.Personality != null) ? d.Personality : "";
            int rel  = (d != null) ? d.Reputation : 0;
            PersonalityProfile prof = (d != null) ? GetProfile(d.Personality) : null;
            double attachment    = (prof != null) ? prof.Attachment     : 0.50;
            double confidence    = (prof != null) ? prof.Confidence     : 0.50;
            double greed         = (prof != null) ? prof.Greed          : 0.50;
            double riskiness     = (prof != null) ? prof.Riskiness      : 0.50;
            bool warm = IsWarmPersonality(d);

            // World context — time via helper
            bool isNight     = IsNight();
            // Weather — Rain(6) and Thunder(7) enum values absent in this SHVDN build; compare as int
            int  wxInt       = (int)World.Weather;
            bool isRain      = wxInt == 6 || wxInt == 7;   // Rain / Thunder
            bool isSnow      = wxInt == 10 || wxInt == 11 || wxInt == 12; // Snowing / Blizzard / Snowlight
            bool isFog       = wxInt == 4;                 // Foggy
            bool isHot       = wxInt == 0 || wxInt == 1;   // ExtraSunny / Clear
            bool isCloudy    = wxInt == 2 || wxInt == 5;   // Clouds / Overcast

            // Update mood based on branch interactions
            if (d != null)
            {
                if (branch == 3) d.Mood = "Playful";
                else if (branch == 4) d.Mood = "Playful"; // Make her Mine — romantic headspace
                else if (branch == 5 && rel < 10) d.Mood = "Alert"; // Personal Stuff at low rep
                else if (branch == 2) { /* mood reveal — don't change it */ }
                else if (warm) d.Mood = "Relaxed";
            }

            switch (branch)
            {
                // ── 0: Get to Know Her ──────────────────────────────────────────────────────
                case 0:
                {
                    // She opens up more as reputation grows: +25% chance per 30 rep (caps at 75% at 90+)
                    // rep/30 is integer division: 0-29→0, 30-59→0.25, 60-89→0.50, 90+→0.75
                    bool opensUp = rng.NextDouble() < Math.Min(0.75, (rel / 30) * 0.25);

                    switch (item)
                    {
                        case 0: // "What's your name?"
                            // Already revealed — deflect regardless of warmth
                            if (d != null && d.NameKnown)
                                return new string[] { "~r~You already know it.", "~r~We've been through this." }[rng.Next(2)];
                            if (warm || opensUp)
                            {
                                if (d != null) d.Reputation += 1;
                                return new string[] { "It's " + (d != null ? d.Name : "none of your business") + ". Nice to meet you.", (d != null ? d.Name : "?") + ". Don't forget it." }[rng.Next(2)];
                            }
                            if (d != null) d.Reputation -= 1;
                            return new string[] { "~r~Why does that matter?", "~r~I don't just give that out." }[rng.Next(2)];
                        case 1: // "What do you do for fun?"
                            if (d != null && (d.KnownTopics & (1 << 0)) != 0)
                            {
                                if (p == "Sarcastic")    return new string[] { "~r~\"We covered this. Pay attention.\"", "~r~\"You already know the answer.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"You asked that.\"", "~r~\"Same answer.\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"Already told you. Were you even listening?\"", "~r~\"I told you. Keep up.\"" }[rng.Next(2)];
                                if (p == "Shy")          return new string[] { "~r~\"I... already said that.\"", "~r~\"I told you already.\"" }[rng.Next(2)];
                                if (p == "Mysterious")   return new string[] { "~r~\"You have your answer.\"", "~r~\"I don't repeat myself much.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already told you that.\"", "~r~\"We've been through this.\"" }[rng.Next(2)];
                            }
                            if (p == "Party Girl") return "\"Clubs, mostly. Dancing. Sometimes I just drive around with music loud.\"";
                            if (p == "Romantic")   return "\"Honestly? Long walks. Reading. Cooking for someone.\"";
                            if (p == "Playful")    return "\"Causing problems, mostly. You'd love it.\"";
                            if ((p == "Cold" || p == "Independent") && !opensUp) return "~r~\"Why do you care?\"";
                            if (p == "Mysterious") return "\"Things you probably wouldn't expect.\"";
                            return (warm || opensUp) ? "\"Nothing special. Just getting through the day.\"" : "~r~\"That's personal.\"";
                        case 2: // "You from around here?"
                            if (d != null && (d.KnownTopics & (1 << 1)) != 0)
                            {
                                if (p == "Sarcastic")    return new string[] { "~r~\"You really don't listen, do you?\"", "~r~\"We just went over this.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"I answered that.\"", "~r~\"We're done with that topic.\"" }[rng.Next(2)];
                                if (p == "Street Smart") return new string[] { "~r~\"I told you already. Move on.\"", "~r~\"You've got a short memory.\"" }[rng.Next(2)];
                                if (p == "Chaotic")      return new string[] { "~r~\"Seriously? We talked about this.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked me that.\"", "~r~\"I already answered that.\"" }[rng.Next(2)];
                            }
                            if (p == "Street Smart") return "\"Born and raised. I know every corner.\"";
                            if (p == "Classy")       return "\"I've been here long enough. It's... fine.\"";
                            if (p == "Chaotic")      return "\"Moved around a lot. Never stayed anywhere long.\"";
                            return (warm || opensUp) ? "\"Yeah. Why?\"" : "~r~\"Does it matter?\"";
                        case 3: // "What kind of guys do you like?"
                            if (d != null && (d.KnownTopics & (1 << 2)) != 0)
                            {
                                if (p == "Sarcastic")    return new string[] { "~r~\"I told you. Take notes next time.\"", "~r~\"You already have my answer.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"We covered that.\"", "~r~\"I'm not saying it again.\"" }[rng.Next(2)];
                                if (p == "Dominant")     return new string[] { "~r~\"I gave you that information already.\"", "~r~\"You should have been paying attention.\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"You already know the answer. Do better.\"", "~r~\"Pay attention. I already said this.\"" }[rng.Next(2)];
                                if (p == "Romantic")     return new string[] { "~r~\"I already told you what I'm looking for.\"", "~r~\"You know what I said.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You know the answer already.\"", "~r~\"We've done this.\"" }[rng.Next(2)];
                            }
                            if (p == "Dominant")     return "\"Men who don't need me to hold their hand.\"";
                            if (p == "Gold Digger")  return "\"Successful ones. Is that so wrong?\"";
                            if (p == "Romantic")     return "\"Someone who actually listens. You'd be surprised how rare that is.\"";
                            if (p == "Sarcastic")    return "\"Ha. Ones who don't ask that question, usually.\"";
                            if (p == "Playful")      return "\"Funny ones. Life's too short for boring.\"";
                            if (p == "Cold" && !opensUp) return "~r~\"I don't.\"";
                            return (warm || opensUp) ? "\"Honest ones, mostly.\"" : "~r~\"Not sure why that's your business.\"";
                        case 4: // "What are you looking for?"
                            if (d != null && (d.KnownTopics & (1 << 3)) != 0)
                            {
                                if (p == "依賴")        return new string[] { "~r~\"I already said. Please don't make me repeat it.\"", "~r~\"We talked about this already.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"Answered.\"", "~r~\"I said it once.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Didn't we already cover this?\"", "~r~\"Still the same answer.\"" }[rng.Next(2)];
                                if (p == "Independent")  return new string[] { "~r~\"You already know. Move on.\"", "~r~\"We went over this.\"" }[rng.Next(2)];
                                if (p == "Gold Digger")  return new string[] { "~r~\"My standards haven't changed.\"", "~r~\"I think I was clear the first time.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already told you.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                            }
                            if (p == "依賴")        return "\"Someone to stick around. Is that too much to ask?\"";
                            if (p == "Independent")  return "\"Nothing serious. Just... company, sometimes.\"";
                            if (p == "Gold Digger")  return "\"Security. Comfort. Someone worth my time.\"";
                            if (p == "Romantic")     return "\"Something real. I know, I know.\"";
                            if (p == "Chaotic")      return "\"No idea. That's kind of the fun part.\"";
                            if (p == "Cold" && !opensUp) return "~r~\"Not this conversation.\"";
                            return (warm || opensUp) ? "\"Still figuring that out.\"" : "~r~\"Nothing you can offer.\"";
                        case 5: // "You seeing anyone?"
                            if (d != null && (d.KnownTopics & (1 << 4)) != 0)
                            {
                                if (p == "嫉妒")      return new string[] { "~r~\"You already asked. Why are you asking again?\"", "~r~\"I said no. Are you suspicious of something?\"" }[rng.Next(2)];
                                if (p == "Manipulative") return new string[] { "~r~\"I gave you my answer.\"", "~r~\"Still complicated. Still not your business.\"" }[rng.Next(2)];
                                if (p == "依賴")        return new string[] { "~r~\"I told you. Still no.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"Still none of your business.\"", "~r~\"Answered.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Nothing changed in the last five minutes.\"", "~r~\"Asked and answered.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already answered that.\"", "~r~\"We've been through this.\"" }[rng.Next(2)];
                            }
                            if (p == "嫉妒")      return "\"...Not anymore. And I'd rather not talk about it.\"";
                            if (p == "依賴")        return "\"No. And it's been a while.\"";
                            if (p == "Manipulative") return "\"It's complicated. Why do you ask?\"";
                            if (p == "Cold" && !opensUp) return "~r~\"That's none of your business.\"";
                            return (warm || opensUp) ? "\"No. Not right now.\"" : "~r~\"Why are you asking?\"";
                        case 6: // "What's your story?"
                            if (d != null && (d.KnownTopics & (1 << 5)) != 0)
                            {
                                if (p == "Mysterious")   return new string[] { "~r~\"You already have more than most people get.\"", "~r~\"I don't retell stories.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"We've done this chapter already.\"", "~r~\"Still the same story. Try a new question.\"" }[rng.Next(2)];
                                if (p == "Unstable")     return new string[] { "~r~\"I already gave you a version of it.\"", "~r~\"The story doesn't change just because you ask again.\"" }[rng.Next(2)];
                                if (p == "Sweet")        return new string[] { "~r~\"I already told you. Nothing changed.\"", "~r~\"Ask me something new.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"I said what I said.\"", "~r~\"No.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"Come on, it's not that deep.\"", "~r~\"Ask me something new.\"" }[rng.Next(2)];
                            }
                            if (p == "Mysterious")   return "\"That would take too long. And you probably couldn't handle it.\"";
                            if (p == "Unstable")     return "\"Which version do you want?\"";
                            if (p == "Sarcastic")    return "\"Oh, you know. Girl grows up, world disappoints her, here we are.\"";
                            if (p == "Sweet")        return "\"Nothing dramatic. Just trying to be happy.\"";
                            if (p == "Cold" && !opensUp) return "~r~\"I don't do autobiography.\"";
                            return (warm || opensUp) ? "\"Nothing special. Ask me something specific.\"" : "~r~\"I don't really do that.\"";
                    }
                    break;
                }

                // ── 3: Flirt ────────────────────────────────────────────────────────────────
                case 3:
                {
                    // Friendzoned: she sees you as a friend — all flirt attempts deflected
                    if (d != null && d.Relationship == "Friendzoned")
                    {
                        string[] fzLines = {
                            "~r~\"I care about you. Just not like that.\"",
                            "~r~\"You're sweet. I don't see you that way though.\"",
                            "~r~\"Please don't make this weird. I like having you around.\"",
                            "~r~\"I think of you as a friend. Let's keep it at that.\"",
                            "~r~\"Don't ruin what we have.\"",
                            "~r~\"You know I care about you. Just not in that way.\"",
                            "~r~\"I've told you already. I don't feel that way.\"",
                        };
                        return fzLines[rng.Next(fzLines.Length)];
                    }
                    switch (item)
                    {
                        case 0: // Compliment her looks
                            if (d != null && (d.KnownTopics & (1 << 17)) != 0)
                            {
                                if (p == "Cold" || p == "Sarcastic") return new string[] { "~r~\"You already said that. Still not impressed.\"", "~r~\"Heard it.\"" }[rng.Next(2)];
                                if (p == "Dominant")     return new string[] { "~r~\"You already told me. Once was enough.\"", "~r~\"I heard you the first time.\"" }[rng.Next(2)];
                                if (p == "Shy")          return new string[] { "~r~\"You... you already said that.\"", "~r~\"You're repeating yourself.\"" }[rng.Next(2)];
                                if (p == "依賴")        return new string[] { "~r~\"You already said that. Not that I mind.\"", "~r~\"I heard you the first time.\"" }[rng.Next(2)];
                                if (p == "Manipulative") return new string[] { "~r~\"You already went there. Push harder.\"", "~r~\"You said that already.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already said that.\"", "~r~\"I heard you the first time.\"", "~r~\"You're repeating yourself.\"" }[rng.Next(3)];
                            }
                            if (p == "Classy" || p == "Romantic")
                                return new string[] { "\"Thank you. That's actually sweet.\"", "\"That's genuinely nice of you.\"", "\"You noticed. I appreciate that.\"" }[rng.Next(3)];
                            if (p == "Cold" || p == "Sarcastic")
                                return new string[] { "~r~\"I know. Was that supposed to do something?\"", "~r~\"Okay. And?\"", "~r~\"I'm aware.\"", "~r~\"Thanks. Moving on.\"" }[rng.Next(4)];
                            if (p == "Manipulative")
                                return new string[] { "\"Mmm. Keep going.\"", "\"You're good at this.\"", "\"That's a start.\"" }[rng.Next(3)];
                            if (p == "Dominant")
                                return new string[] { "\"Good observation. Now what?\"", "\"Finally. I was wondering when you'd say something.\"", "\"About time.\"" }[rng.Next(3)];
                            if (p == "Shy")
                                return new string[] { "\"Oh... thank you.\"", "\"I— thanks. That means a lot.\"", "\"Really? Wow, thank you.\"" }[rng.Next(3)];
                            if (p == "依賴")
                                return new string[] { "\"You always know what to say.\"", "\"Thank you. I needed that.\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"Ha, thank you.\"", "\"That's sweet.\"", "\"You're not bad yourself.\"", "\"I appreciate that.\"" }[rng.Next(4)]
                                : new string[] { "~r~\"Thanks. And?\"", "~r~\"Okay.\"", "~r~\"Yeah, I know.\"" }[rng.Next(3)];
                        case 1: // Compliment her vibe
                            if (d != null && (d.KnownTopics & (1 << 18)) != 0)
                            {
                                if (p == "Cold")         return new string[] { "~r~\"You already said that. My vibe hasn't changed.\"", "~r~\"Still don't know what you mean by that.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You're still on my 'vibe.' Really.\"", "~r~\"We covered the vibe. What else?\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"Ha, you already told me that! Pick a new line.\"", "~r~\"You already tried that one.\"" }[rng.Next(2)];
                                if (p == "Shy")          return new string[] { "~r~\"You... already said that.\"", "~r~\"You told me already.\"" }[rng.Next(2)];
                                if (p == "Mysterious")   return new string[] { "~r~\"You already observed that. Don't repeat yourself.\"", "~r~\"Heard it.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already tried that.\"", "~r~\"Heard it.\"", "~r~\"Same line again?\"" }[rng.Next(3)];
                            }
                            if (p == "Mysterious")
                                return new string[] { "\"That's... actually a new one. I'll take it.\"", "\"Most people don't pick up on that. You're observant.\"", "\"Interesting that you see that.\"" }[rng.Next(3)];
                            if (p == "Cold")
                                return new string[] { "~r~\"My vibe is 'leave me alone.' You're not reading it well.\"", "~r~\"Thanks.\"", "~r~\"Didn't ask.\"" }[rng.Next(3)];
                            if (p == "Playful")
                                return new string[] { "\"I know, right? I'm a lot.\"", "\"Ha, you noticed!\"", "\"I try.\"" }[rng.Next(3)];
                            if (p == "Sarcastic")
                                return new string[] { "\"Sure, my 'vibe.' Very specific.\"", "\"What does that even mean?\"", "\"Interesting choice of words.\"" }[rng.Next(3)];
                            if (p == "Shy")
                                return new string[] { "\"Nobody's ever said that before.\"", "\"I don't really... thank you.\"" }[rng.Next(2)];
                            if (p == "Party Girl")
                                return new string[] { "\"Ha! Right? I bring the energy.\"", "\"I'm told that a lot.\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"You noticed. I like that.\"", "\"Thanks. That's actually a compliment.\"", "\"Ha. You're perceptive.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"Alright.\"", "~r~\"Okay.\"", "~r~\"Sure.\"" }[rng.Next(3)];
                        case 2: // Be bold
                            if (d != null && (d.KnownTopics & (1 << 19)) != 0)
                            {
                                if (p == "Dominant")     return new string[] { "~r~\"You already went bold. Don't repeat yourself.\"", "~r~\"You said that already. Come up with something new.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Bold. Again. Less impressive the second time.\"", "~r~\"You already did the bold thing.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"You already went there. Didn't work. Try something else.\"", "~r~\"Still too much. Still a no.\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"You already tried that! It was better the first time.\"", "~r~\"Still with this? Try harder.\"" }[rng.Next(2)];
                                if (p == "Shy")          return new string[] { "~r~\"You already did that... still too fast.\"", "~r~\"You already went there. Please slow down.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"Still with this?\"", "~r~\"You already went there.\"", "~r~\"Try something new.\"" }[rng.Next(3)];
                            }
                            if (p == "Dominant")
                                return new string[] { "\"Ha. Finally. I was wondering when you'd grow a spine.\"", "\"There it is. Now we're getting somewhere.\"", "\"Bold. I like it.\"" }[rng.Next(3)];
                            if (p == "Shy")
                                return new string[] { "~r~\"That's... a lot. I need a second.\"", "~r~\"Oh wow. I wasn't ready for that.\"", "~r~\"Can you slow down a little?\"" }[rng.Next(3)];
                            if (p == "Aggressive")
                                return new string[] { "\"Now we're talking.\"", "\"Good. I don't like slow.\"", "\"Finally. Get to the point.\"" }[rng.Next(3)];
                            if (p == "Romantic")
                                return new string[] { "~r~\"Easy. That's a bit much.\"", "~r~\"I need to know you first.\"", "~r~\"Slow down a little.\"" }[rng.Next(3)];
                            if (p == "Sarcastic")
                                return new string[] { "\"Bold. Noted. We'll see.\"", "\"Points for confidence, at least.\"", "\"Okay. That happened.\"" }[rng.Next(3)];
                            if (p == "Cold")
                                return new string[] { "~r~\"Too much.\"", "~r~\"Dial it back.\"", "~r~\"Okay, relax.\"" }[rng.Next(3)];
                            if (p == "Playful")
                                return new string[] { "\"Oh okay. We're doing this.\"", "\"Ha! I like this side of you.\"", "\"There we go. Finally.\"" }[rng.Next(3)];
                            return warm
                                ? new string[] { "\"Okay. You've got nerve.\"", "\"Ha. Alright then.\"", "\"I wasn't expecting that.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"Too much.\"", "~r~\"Slow down.\"", "~r~\"Not like that.\"" }[rng.Next(3)];
                        case 3: // "I think about you more than I should."
                            if (d == null || d.Reputation < 10) return "~r~\"...I don't even know what that means.\"";
                            if (d != null && (d.KnownTopics & (1 << 20)) != 0)
                            {
                                if (p == "Shy" || p == "Sweet" || p == "Romantic") return new string[] { "~r~\"You already said that. It was sweet once.\"", "~r~\"You told me. I remember.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You literally just said that.\"", "~r~\"You already went with that line.\"" }[rng.Next(2)];
                                if (p == "依賴")        return new string[] { "~r~\"You already told me. I believed you the first time.\"", "~r~\"You said that. I heard you.\"" }[rng.Next(2)];
                                if (p == "Gold Digger")  return new string[] { "~r~\"You already said that. It didn't help your case.\"", "~r~\"Still sweet. Still doesn't pay rent.\"" }[rng.Next(2)];
                                if (p == "Dominant" || p == "Aggressive") return new string[] { "~r~\"You said that once. Don't go soft on me.\"", "~r~\"Already heard that.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You said that already.\"", "~r~\"I know. You told me.\"", "~r~\"Find something new to say.\"" }[rng.Next(3)];
                            }
                            if (p == "Shy" || p == "Sweet" || p == "Romantic")
                                return new string[] { "\"Aw. That's genuinely nice.\"", "\"That's the sweetest thing.\"", "\"You're going to make me blush.\"" }[rng.Next(3)];
                            if (p == "Dominant" || p == "Aggressive")
                                return new string[] { "~r~\"Don't get soft on me.\"", "~r~\"Save the sweet stuff.\"", "~r~\"I prefer direct.\"" }[rng.Next(3)];
                            if (p == "Sarcastic")
                                return new string[] { "\"Are you always like this or just with strangers?\"", "\"Interesting tactic.\"", "\"Hm. Different.\"" }[rng.Next(3)];
                            if (p == "Gold Digger")
                                return new string[] { "\"Sweet doesn't pay rent, but sure.\"", "\"That's nice. What else you got?\"" }[rng.Next(2)];
                            if (p == "依賴")
                                return new string[] { "\"Keep talking like that.\"", "\"I needed to hear that.\"", "\"You're really sweet, you know that?\"" }[rng.Next(3)];
                            if (p == "Playful")
                                return new string[] { "\"Aw, who knew you had a soft side.\"", "\"Ha. That was actually sweet.\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"That's sweet. I mean it.\"", "\"Aw. Thank you.\"", "\"That actually got to me a little.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"Didn't take you for that type.\"", "~r~\"Okay.\"", "~r~\"Right.\"" }[rng.Next(3)];
                        case 4: // Tease her
                            if (d != null && (d.KnownTopics & (1 << 21)) != 0)
                            {
                                if (p == "Playful")      return new string[] { "~r~\"Ha, you already did that. Getting lazy?\"", "~r~\"That worked once. Not twice.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You already tried that move.\"", "~r~\"That joke got stale.\"" }[rng.Next(2)];
                                if (p == "嫉妒")      return new string[] { "~r~\"I told you I don't like games. Do it again and I'm done.\"", "~r~\"You're still doing this. I don't like it.\"" }[rng.Next(2)];
                                if (p == "Aggressive")   return new string[] { "~r~\"You already tried that. Not impressed twice.\"", "~r~\"Same move. Still watching.\"" }[rng.Next(2)];
                                if (p == "Chaotic")      return new string[] { "~r~\"Ha, you already used that. Come up with something new.\"", "~r~\"That one's expired.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"That joke got old.\"", "~r~\"Again? Really?\"", "~r~\"You already did that.\"" }[rng.Next(3)];
                            }
                            if (p == "Playful")
                                return new string[] { "\"Oh you are asking for it, I hope you know that.\"", "\"Oh that's how it is? Game on.\"", "\"I love this. Keep going.\"" }[rng.Next(3)];
                            if (p == "Sarcastic")
                                return new string[] { "\"You're going to lose this. Fair warning.\"", "\"That's brave.\"", "\"Oh I can do this all day.\"" }[rng.Next(3)];
                            if (p == "嫉妒")
                                return new string[] { "~r~\"Watch it. I don't like games.\"", "~r~\"Don't tease me.\"", "~r~\"I'm not in the mood for this.\"" }[rng.Next(3)];
                            if (p == "Aggressive")
                                return new string[] { "~r~\"Try me. See what happens.\"", "~r~\"You don't want that.\"", "~r~\"Wrong move.\"" }[rng.Next(3)];
                            if (p == "Shy")
                                return new string[] { "~r~\"Please don't.\"", "~r~\"That's not really funny.\"", "~r~\"Stop it.\"" }[rng.Next(3)];
                            if (p == "Cold")
                                return new string[] { "~r~\"That's not funny.\"", "~r~\"Mature.\"", "~r~\"Really?\"" }[rng.Next(3)];
                            if (p == "Chaotic")
                                return new string[] { "\"Oh you want trouble? I am trouble.\"", "\"Ha! I like you.\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"Ha. Okay, I see you.\"", "\"You're trouble.\"", "\"Don't start something you can't finish.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"That's not funny.\"", "~r~\"Really?\"", "~r~\"Pass.\"" }[rng.Next(3)];
                        case 5: // "Did you miss me?"
                            if (d == null || d.Reputation < 10) return "~r~\"Did I miss you? We've never even met properly.\"";
                            if (d != null && (d.KnownTopics & (1 << 22)) != 0)
                            {
                                if (p == "依賴")        return new string[] { "~r~\"You already asked that. Still a little.\"", "~r~\"You already know the answer.\"" }[rng.Next(2)];
                                if (p == "Independent")  return new string[] { "~r~\"Still no. You already asked.\"", "~r~\"You asked. I answered. Same answer.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"You already asked that. No.\"", "~r~\"We covered this.\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"Ha, fishing again? You already asked.\"", "~r~\"You already tried that. I'm not telling.\"" }[rng.Next(2)];
                                if (p == "嫉妒")      return new string[] { "~r~\"You asked. Now you're asking again. What do you want to hear?\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You really need the validation that bad?\"", "~r~\"Still asking. Still not telling.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked that.\"", "~r~\"I already answered.\"", "~r~\"You really need the validation, huh?\"" }[rng.Next(3)];
                            }
                            if (p == "依賴")
                                return new string[] { "\"...Yes. A little. Don't make it weird.\"", "\"More than I should have.\"", "\"Kind of a lot, actually.\"" }[rng.Next(3)];
                            if (p == "Independent")
                                return new string[] { "~r~\"I stay busy. So. No.\"", "~r~\"I don't really do that.\"", "~r~\"Not particularly.\"" }[rng.Next(3)];
                            if (p == "Cold")
                                return new string[] { "~r~\"No.\"", "~r~\"Not really.\"", "~r~\"I don't miss people.\"" }[rng.Next(3)];
                            if (p == "Playful")
                                return new string[] { "\"Maybe. Ask me again later.\"", "\"A little. Don't let it go to your head.\"", "\"I'll never tell.\"" }[rng.Next(3)];
                            if (p == "Romantic")
                                return new string[] { "\"I thought about you, yeah.\"", "\"More than I'd admit to most people.\"", "\"Yeah. I did.\"" }[rng.Next(3)];
                            if (p == "Sarcastic")
                                return new string[] { "\"Define 'miss.'\"", "\"I mean, I noticed you weren't around.\"", "\"Bold of you to assume.\"" }[rng.Next(3)];
                            if (p == "嫉妒")
                                return new string[] { "\"...Maybe. Where were you?\"", "\"Why? Were you with someone?\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"A bit. Yeah.\"", "\"I noticed you weren't here.\"", "\"Maybe a little.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"Not really.\"", "~r~\"I was fine.\"", "~r~\"I don't keep track.\"" }[rng.Next(3)];
                        case 6: // "You look good tonight/today."
                            if (d != null && (d.KnownTopics & (1 << 23)) != 0)
                            {
                                if (p == "Classy")       return new string[] { "~r~\"You already said that. I know.\"", "~r~\"You told me. Once was sufficient.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"You already said that. Okay.\"", "~r~\"I know. You mentioned it.\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"Ha, you already told me! Still true though.\"", "~r~\"You already said it. I accepted.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You said that already. Keep them coming.\"", "~r~\"You mentioned it. Still obvious.\"" }[rng.Next(2)];
                                if (p == "Shy")          return new string[] { "~r~\"You... you already said that. Thank you again.\"", "~r~\"You already told me that.\"" }[rng.Next(2)];
                                if (p == "Romantic")     return new string[] { "~r~\"You already said that. You mean it, I know.\"", "~r~\"I heard you the first time.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already said that.\"", "~r~\"Thanks. Again.\"", "~r~\"You mentioned it.\"" }[rng.Next(3)];
                            }
                            if (p == "Classy")
                                return new string[] { "\"I know. Thank you for noticing.\"", "\"Always do. But thank you.\"", "\"I try. But yes, thank you.\"" }[rng.Next(3)];
                            if (p == "Cold")
                                return new string[] { "~r~\"Thanks.\"", "~r~\"I know.\"", "~r~\"Okay.\"" }[rng.Next(3)];
                            if (p == "Playful")
                                return isNight
                                    ? new string[] { "\"I always look good at night. What else?\"", "\"Night suits me. I know.\"", "\"Thank you. Night's my time.\"" }[rng.Next(3)]
                                    : new string[] { "\"I always look good. What else?\"", "\"Took you long enough to say it.\"", "\"Just catching up.\"" }[rng.Next(3)];
                            if (p == "Shy")
                                return new string[] { "\"Oh... thank you.\"", isNight ? "\"Really? I wasn't sure about tonight.\"" : "\"Really? I wasn't sure.\"", "\"That's really kind.\"" }[rng.Next(3)];
                            if (p == "Sarcastic")
                                return new string[] { "\"Obviously.\"", "\"Just now noticing?\"", "\"I was wondering when you'd say something.\"" }[rng.Next(3)];
                            if (p == "Romantic")
                                return isNight
                                    ? new string[] { "\"Thank you. Night looks good on you too.\"", "\"That's sweet. You're not bad yourself.\"" }[rng.Next(2)]
                                    : new string[] { "\"Thank you. That's kind.\"", "\"That's sweet of you.\"" }[rng.Next(2)];
                            return warm
                                ? (isNight
                                    ? new string[] { "\"Thank you. Night looks good on you too.\"", "\"Good night for it.\"", "\"Thank you. You're sweet.\"" }[rng.Next(3)]
                                    : new string[] { "\"Thank you. That's kind.\"", "\"That's nice of you.\"", "\"Ha, thanks.\"" }[rng.Next(3)])
                                : new string[] { "~r~\"Yeah. Okay.\"", "~r~\"Thanks.\"", "~r~\"Sure.\"" }[rng.Next(3)];
                        case 7: // "I love you."
                            // ── GF + Rep > 80: she says it back ──────────────────────────────────
                            if (d != null && d.Relationship == "Girlfriend" && d.Reputation > 80)
                            {
                                if (p == "Romantic")  return new string[] { "\"I love you too. I love you.\"", "\"I was waiting for you to say that.\"", "\"Those words from you... yes. Yes.\"" }[rng.Next(3)];
                                if (p == "依賴")     return new string[] { "\"I love you so much. You have no idea.\"", "\"I needed to hear that.\"", "\"Don't you ever stop saying that.\"" }[rng.Next(3)];
                                if (p == "Shy")       return new string[] { "\"I... I love you too.\"", "\"You know I do. Don't make me say it out loud.\"", "\"Yeah. I do.\"" }[rng.Next(3)];
                                if (p == "Playful")   return new string[] { "\"Took you long enough!\"", "\"Ha. I know. I love you too.\"", "\"Finally. Geez.\"" }[rng.Next(3)];
                                if (p == "Cold")      return new string[] { "\"...I know.\"", "\"Yeah.\"", "\"I hear you.\"" }[rng.Next(3)];
                                if (p == "Sarcastic") return new string[] { "\"Wow. Did you rehearse that?\"", "\"Look at you. Actually saying it.\"", "\"...I love you too. Don't make it weird.\"" }[rng.Next(3)];
                                return new string[] { "\"I love you too.\"", "\"That means a lot. I love you too.\"", "\"...Yeah. I love you.\"" }[rng.Next(3)];
                            }
                            // ── GF but rep not high enough: touched but not ready ────────────────
                            if (d != null && d.Relationship == "Girlfriend")
                            {
                                if (p == "Romantic")  return new string[] { "~r~\"I care about you so much. I just need a little more time.\"", "~r~\"That means everything. I'm just not ready to say it back yet.\"" }[rng.Next(2)];
                                if (p == "依賴")     return new string[] { "~r~\"You mean so much to me. I just... not yet.\"", "~r~\"I really like you. Please don't rush me.\"" }[rng.Next(2)];
                                if (p == "Shy")       return new string[] { "~r~\"I... that's a lot. Give me time.\"", "~r~\"I'm not there yet. But I want to be.\"" }[rng.Next(2)];
                                if (p == "Sarcastic") return new string[] { "~r~\"Wow. Okay. I'm not... I can't say that back yet.\"", "~r~\"Don't rush it. I need to actually feel ready.\"" }[rng.Next(2)];
                                if (p == "Cold")      return new string[] { "~r~\"...Not yet. But I hear you.\"", "~r~\"Don't push that. I'll get there.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I care about you. I'm just not ready to say that.\"", "~r~\"That means a lot. I'm just... not there yet.\"", "~r~\"I need a little more time. Don't take it the wrong way.\"" }[rng.Next(3)];
                            }
                            // ── Not GF: personality-based rejection ──────────────────────────────
                            if (p == "Cold")        return new string[] { "~r~\"Don't say that.\"", "~r~\"No. Stop.\"", "~r~\"That's not what this is.\"" }[rng.Next(3)];
                            if (p == "Sarcastic")   return new string[] { "~r~\"Wow. No. Absolutely not.\"", "~r~\"...Okay. No. Just no.\"", "~r~\"That's way too much.\"" }[rng.Next(3)];
                            if (p == "Aggressive")  return new string[] { "~r~\"What the hell? Back off.\"", "~r~\"You don't even know me. Stop.\"", "~r~\"Too far. Way too far.\"" }[rng.Next(3)];
                            if (p == "Independent") return new string[] { "~r~\"I barely know you. Please stop.\"", "~r~\"That's not something I can say back.\"", "~r~\"I need you to pump the brakes.\"" }[rng.Next(3)];
                            if (p == "Dominant")    return new string[] { "~r~\"That's way too much. Pump the brakes.\"", "~r~\"We're not there. Not even close.\"", "~r~\"Don't say that to me. Not yet.\"" }[rng.Next(3)];
                            if (p == "Shy")         return new string[] { "~r~\"I— that's a lot. I'm not there.\"", "~r~\"I can't... that's too big right now.\"", "~r~\"That's... a lot. I'm not ready.\"" }[rng.Next(3)];
                            if (p == "Mysterious")  return new string[] { "~r~\"That's not where we are.\"", "~r~\"Don't go there.\"", "~r~\"...No.\"" }[rng.Next(3)];
                            if (p == "Romantic")    return new string[] { "~r~\"I want to feel that. I just... don't yet. I'm sorry.\"", "~r~\"I wish I could say it back. I'm not there yet.\"", "~r~\"That means everything. I just... need more time.\"" }[rng.Next(3)];
                            if (p == "依賴")       return new string[] { "~r~\"That's... really sweet. I'm not sure I'm there yet.\"", "~r~\"I— wow. I really like you. Just not... yet.\"", "~r~\"Please don't be upset. I'm just not ready.\"" }[rng.Next(3)];
                            return new string[] { "~r~\"That's too much. I'm not ready for that.\"", "~r~\"Slow down. We're not there.\"", "~r~\"I can't say that back. I'm sorry.\"" }[rng.Next(3)];
                    }
                    break;
                }

                // ── 4: Make Her Mine ────────────────────────────────────────────────────────
                case 4:
                {
                    // Gate: only meaningful once she's Flirty (or already Girlfriend)
                    if (d == null || (d.Relationship != "Flirty" && d.Relationship != "Girlfriend"))
                    {
                        if (d != null && d.Relationship == "Friendzoned")
                        {
                            string[] fzMhmLines = {
                                "~r~\"I already told you. I don't think of you that way.\"",
                                "~r~\"Please stop. You're making this awkward.\"",
                                "~r~\"I value what we have. Don't go there.\"",
                                "~r~\"That's not going to happen. I'm sorry.\"",
                            };
                            return fzMhmLines[rng.Next(fzMhmLines.Length)];
                        }
                        if (rel < 10)
                        {
                            string[] coldLines = { "~r~\"I don't even know you.\"", "~r~\"We've barely spoken.\"", "~r~\"That's way too fast.\"" };
                            return coldLines[rng.Next(coldLines.Length)];
                        }
                        string[] notReadyLines = {
                            "~r~\"I'm not ready for that kind of conversation.\"",
                            "~r~\"It's too soon.\"",
                            "~r~\"We're not there yet.\"",
                            "~r~\"I need more time before we talk like this.\"",
                        };
                        return notReadyLines[rng.Next(notReadyLines.Length)];
                    }
                    if (d.Relationship == "Girlfriend")
                    {
                        // Break Up responses — keyed by item
                        switch (item)
                        {
                            case 0: // "I need some space."
                                if (p == "依賴")      return new string[] { "~r~\"Space? What did I do?\"", "~r~\"Please don't do this.\"" }[rng.Next(2)];
                                if (p == "Cold")       return new string[] { "~r~\"Fine. Take it.\"", "~r~\"Okay.\"" }[rng.Next(2)];
                                if (p == "嫉妒")    return new string[] { "~r~\"Space from me or from someone else?\"", "~r~\"You're not going anywhere without telling me why.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"Okay. I hear you.\"", "~r~\"That hurts, but alright.\"", "~r~\"Fine. Space.\"" }[rng.Next(3)];
                            case 1: // "This isn't working."
                                if (p == "Romantic")   return new string[] { "~r~\"How can you say that?\"", "~r~\"I thought this meant something to you.\"" }[rng.Next(2)];
                                if (p == "依賴")      return new string[] { "~r~\"We can fix it. We can fix anything.\"", "~r~\"Please. Tell me what's wrong.\"" }[rng.Next(2)];
                                if (p == "Cold")       return new string[] { "~r~\"Figured.\"", "~r~\"Yeah. I know.\"" }[rng.Next(2)];
                                if (p == "Aggressive") return new string[] { "~r~\"You're ending this? Seriously?\"", "~r~\"That's not your call.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"That's not what I expected to hear.\"", "~r~\"Okay. I'm not going to beg.\"", "~r~\"That stings.\"" }[rng.Next(3)];
                            case 2: // "I think we should end this."
                                if (p == "Shy")        return new string[] { "~r~\"...Is it something I did?\"", "~r~\"I should have seen this coming.\"" }[rng.Next(2)];
                                if (p == "Romantic")   return new string[] { "~r~\"Don't. Please don't.\"", "~r~\"I gave you everything.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")  return new string[] { "~r~\"Classic.\"", "~r~\"Wow. Really?\"" }[rng.Next(2)];
                                if (p == "Manipulative") return new string[] { "~r~\"You'll regret this.\"", "~r~\"Good luck finding someone like me.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"Okay. If that's really how you feel.\"", "~r~\"I'm not going to pretend this doesn't hurt.\"", "~r~\"Alright.\"" }[rng.Next(3)];
                            case 3: // "I don't feel the same anymore."
                                if (p == "依賴")      return new string[] { "~r~\"When did that change? Why didn't you tell me?\"", "~r~\"I've been trying so hard.\"" }[rng.Next(2)];
                                if (p == "Cold")       return new string[] { "~r~\"I saw this coming.\"", "~r~\"Fine.\"" }[rng.Next(2)];
                                if (p == "嫉妒")    return new string[] { "~r~\"Is there someone else?\"", "~r~\"You're lying.\"" }[rng.Next(2)];
                                if (p == "Unstable")   return new string[] { "~r~\"You WHAT?\"", "~r~\"After everything?\"" }[rng.Next(2)];
                                return new string[] { "~r~\"That's honest. Doesn't make it easier.\"", "~r~\"Okay. I won't pretend I saw that coming.\"", "~r~\"Wow.\"" }[rng.Next(3)];
                            case 4: // "I'm sorry. I can't do this."
                                if (p == "Romantic")   return new string[] { "~r~\"Don't apologize. Just stay.\"", "~r~\"You can. You just won't.\"" }[rng.Next(2)];
                                if (p == "Cold")       return new string[] { "~r~\"Okay.\"", "~r~\"Don't be.\"" }[rng.Next(2)];
                                if (p == "Playful")    return new string[] { "~r~\"Is this a bit? It's not funny.\"", "~r~\"Wait, seriously?\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You don't have to be sorry.\"", "~r~\"Okay. Take care of yourself.\"", "~r~\"I get it. It still hurts.\"" }[rng.Next(3)];
                            case 5: // "It's not you, it's me."
                                if (p == "Sarcastic")  return new string[] { "~r~\"Oh WOW. That line?\"", "~r~\"You're serious? That's your exit?\"" }[rng.Next(2)];
                                if (p == "Independent") return new string[] { "~r~\"You don't need to soften it.\"", "~r~\"Just say what you mean.\"" }[rng.Next(2)];
                                if (p == "依賴")      return new string[] { "~r~\"No. Tell me what I did wrong. Please.\"", "~r~\"That can't be all there is to it.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"That's... an interesting way to say it.\"", "~r~\"That line doesn't make it sting less.\"", "~r~\"Sure. Whatever helps you feel better about it.\"" }[rng.Next(3)];
                            default: // "I think we rushed into this."
                                if (p == "Romantic")   return new string[] { "~r~\"I didn't rush. I was sure.\"", "~r~\"Don't say that.\"" }[rng.Next(2)];
                                if (p == "Chaotic")    return new string[] { "~r~\"Yeah. Probably. Doesn't mean I wanted it to end.\"", "~r~\"I knew it. I always do this.\"" }[rng.Next(2)];
                                if (p == "Cold")       return new string[] { "~r~\"Maybe.\"", "~r~\"I don't disagree.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"Maybe you're right. Doesn't make it any easier.\"", "~r~\"I thought we were building something.\"", "~r~\"Okay. I hear you.\"" }[rng.Next(3)];
                        }
                    }
                    switch (item)
                    {
                        case 0: // "I want more than this."
                            if (p == "Independent")
                                return new string[] { "~r~\"That's... a lot. I need time.\"", "~r~\"I move at my own pace.\"", "~r~\"Don't pressure me.\"" }[rng.Next(3)];
                            if (p == "Romantic")
                                return new string[] { "\"I was hoping you'd say that.\"", "\"I've been thinking the same thing.\"", "\"That's exactly what I wanted to hear.\"" }[rng.Next(3)];
                            if (p == "Cold")
                                return new string[] { "~r~\"Don't push it.\"", "~r~\"Slow down.\"", "~r~\"We'll see.\"" }[rng.Next(3)];
                            if (p == "依賴")
                                return new string[] { "\"Say it again. I need to hear it again.\"", "\"I've been waiting for you to say that.\"", "\"Oh thank God.\"" }[rng.Next(3)];
                            if (p == "Shy")
                                return new string[] { "\"I... I'd like that too.\"", "\"Really? You mean it?\"" }[rng.Next(2)];
                            if (p == "Playful")
                                return new string[] { "\"Oh yeah? How much more?\"", "\"Now we're talking.\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"Maybe. Let's not rush it.\"", "\"I feel the same way.\"", "\"Good. Me too.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"Slow down.\"", "~r~\"That's a big step.\"", "~r~\"Give me time.\"" }[rng.Next(3)];
                        case 1: // "Would you be mine?"
                            if (p == "Romantic")
                                return new string[] { "\"Please, yes.\"", "\"Yes. Obviously. Yes.\"", "\"I thought you'd never ask.\"" }[rng.Next(3)];
                            if (p == "Sarcastic")
                                return new string[] { "\"Yours? That's old-fashioned. I like it.\"", "\"'Mine?' How retro. But okay.\"" }[rng.Next(2)];
                            if (p == "Cold")
                                return new string[] { "~r~\"I don't belong to anyone.\"", "~r~\"I don't like that word.\"", "~r~\"That's not how this works.\"" }[rng.Next(3)];
                            if (p == "Shy")
                                return new string[] { "\"...Is this real?\"", "\"I... yes. Yes I would.\"" }[rng.Next(2)];
                            if (p == "依賴")
                                return new string[] { "\"Yes. Please. Yes.\"", "\"I've been hoping you'd ask.\"" }[rng.Next(2)];
                            if (p == "Independent")
                                return new string[] { "~r~\"I don't know. That word is a lot.\"", "~r~\"I don't 'belong' to anyone. But... ask again sometime.\"" }[rng.Next(2)];
                            if (p == "Playful")
                                return new string[] { "\"Mmm. Depends what that comes with.\"", "\"Ha. I was wondering when you'd ask.\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"I've been waiting for you to ask.\"", "\"Yeah. I think so.\"", "\"I'd like that.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"I don't know. Give me a minute.\"", "~r~\"That's a lot to answer right now.\"", "~r~\"Don't rush me.\"" }[rng.Next(3)];
                        case 2: // "I've been thinking about us."
                            if (p == "依賴")
                                return new string[] { "\"Me too. Honestly, all the time.\"", "\"I think about it constantly.\"", "\"Same. Every day.\"" }[rng.Next(3)];
                            if (p == "Cold")
                                return new string[] { "~r~\"Don't overthink it.\"", "~r~\"Then stop.\"", "~r~\"It's not that deep.\"" }[rng.Next(3)];
                            if (p == "Mysterious")
                                return new string[] { "\"And? What conclusion did you reach?\"", "\"Interesting. What kind of thoughts?\"" }[rng.Next(2)];
                            if (p == "Romantic")
                                return new string[] { "\"Me too. I'm glad you said it.\"", "\"So have I. More than you know.\"" }[rng.Next(2)];
                            if (p == "Shy")
                                return new string[] { "\"...You have?\"", "\"I didn't know you felt that way.\"" }[rng.Next(2)];
                            if (p == "Sarcastic")
                                return new string[] { "\"Thinking got you somewhere interesting.\"", "\"Careful. Thinking is dangerous.\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"Good thoughts, I hope.\"", "\"Same, honestly.\"", "\"Me too, a little.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"That's a lot of pressure.\"", "~r~\"Don't put that on me.\"", "~r~\"Okay.\"" }[rng.Next(3)];
                        case 3: // "Let me take you somewhere nice."
                            if (p == "Gold Digger")
                                return new string[] { "\"Now you're speaking my language.\"", "\"I was wondering when you'd offer.\"", "\"Define 'nice.'\"" }[rng.Next(3)];
                            if (p == "Romantic")
                                return new string[] { "\"I'd love that more than you know.\"", "\"Yes. Absolutely yes.\"", "\"That sounds amazing.\"" }[rng.Next(3)];
                            if (p == "Independent")
                                return new string[] { "~r~\"I'm not a charity case.\"", "~r~\"I can take myself.\"", "~r~\"What's the catch?\"" }[rng.Next(3)];
                            if (p == "Playful")
                                return new string[] { "\"Nice as in five-star nice, or just-not-terrible nice?\"", "\"I like where this is going.\"" }[rng.Next(2)];
                            if (p == "Shy")
                                return new string[] { "\"I'd really like that.\"", "\"That's... really sweet. Yes.\"" }[rng.Next(2)];
                            if (p == "Sarcastic")
                                return new string[] { "\"Define nice.\"", "\"How nice are we talking?\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"Yeah? I'd like that.\"", "\"I'd love that actually.\"", "\"I'm in.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"We'll see.\"", "~r~\"Maybe.\"", "~r~\"Don't make promises you can't keep.\"" }[rng.Next(3)];
                        case 4: // "You're different from everyone else."
                            if (p == "Sarcastic")
                                return new string[] { "\"Everyone says that. You might actually mean it though.\"", "\"That's a line. A good one, but still a line.\"", "\"I've heard that before. But keep going.\"" }[rng.Next(3)];
                            if (p == "Shy")
                                return new string[] { "\"That's the nicest thing anyone's said to me in a while.\"", "\"I... thank you. Really.\"" }[rng.Next(2)];
                            if (p == "Cold")
                                return new string[] { "~r~\"I know.\"", "~r~\"Obviously.\"", "~r~\"I'm aware.\"" }[rng.Next(3)];
                            if (p == "依賴")
                                return new string[] { "\"You don't know how much I needed to hear that.\"", "\"I've always felt like I don't fit anywhere. You get that.\"" }[rng.Next(2)];
                            if (p == "Romantic")
                                return new string[] { "\"I've been waiting for someone to say that and mean it.\"", "\"You have no idea what that means to me.\"" }[rng.Next(2)];
                            if (p == "Playful")
                                return new string[] { "\"Ha. Yeah I am.\"", "\"Took you long enough to notice.\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"I get that a lot. But coming from you it means something.\"", "\"That's actually really sweet.\"", "\"I hope you mean that.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"Lines like that don't usually work on me.\"", "~r~\"Everyone thinks they're the first to say that.\"", "~r~\"Sure.\"" }[rng.Next(3)];
                        case 5: // "What would it take to make this real?"
                            if (p == "Gold Digger")
                                return new string[] { "\"Effort. And probably money. But mostly effort.\"", "\"Consistency. And a little ambition.\"" }[rng.Next(2)];
                            if (p == "Romantic")
                                return new string[] { "\"Honesty. Just be real with me.\"", "\"Show up. That's it.\"", "\"Just mean what you say.\"" }[rng.Next(3)];
                            if (p == "Independent")
                                return new string[] { "~r~\"I don't know if I want it to be.\"", "~r~\"I need to think about that.\"", "~r~\"That's a big question.\"" }[rng.Next(3)];
                            if (p == "Shy")
                                return new string[] { "\"Just... time. I need to trust it.\"", "\"Patience. I don't rush things.\"" }[rng.Next(2)];
                            if (p == "依賴")
                                return new string[] { "\"Just stay. That's all I want.\"", "\"Promise me you won't disappear.\"" }[rng.Next(2)];
                            if (p == "Sarcastic")
                                return new string[] { "\"Good question. Let me think.\"", "\"More than you think. Less than you're afraid of.\"" }[rng.Next(2)];
                            if (p == "Mysterious")
                                return new string[] { "\"That depends on things I haven't decided yet.\"", "\"Ask me again in a while.\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"Patience. And showing up.\"", "\"Just be consistent with me.\"", "\"Show me you mean it.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"I don't know yet.\"", "~r~\"More than you probably want to give.\"", "~r~\"I'm still figuring that out.\"" }[rng.Next(3)];
                        case 6: // "I think I'm falling for you."
                            if (p == "Romantic")
                                return new string[] { "\"Don't say that unless you mean it. ...Do you mean it?\"", "\"Say that again. Slowly.\"", "\"I was hoping you'd get there.\"" }[rng.Next(3)];
                            if (p == "Cold")
                                return new string[] { "~r~\"Don't.\"", "~r~\"That's a lot.\"", "~r~\"Don't say things like that.\"" }[rng.Next(3)];
                            if (p == "依賴")
                                return new string[] { "\"I've been trying not to say the same thing.\"", "\"I've felt this for a while. I was scared to say it.\"" }[rng.Next(2)];
                            if (p == "Playful")
                                return new string[] { "\"Oh no. Me too. We're both in trouble.\"", "\"Ha. Welcome to the club.\"" }[rng.Next(2)];
                            if (p == "Sarcastic")
                                return new string[] { "\"That's either the sweetest or the stupidest thing I've heard this week.\"", "\"Bold statement. Let's see if it holds.\"" }[rng.Next(2)];
                            if (p == "Shy")
                                return new string[] { "\"I... I didn't know you felt that way.\"", "\"That's a lot. But maybe... me too.\"" }[rng.Next(2)];
                            if (p == "Independent")
                                return new string[] { "~r~\"That's a big statement.\"", "~r~\"I don't know what to do with that.\"" }[rng.Next(2)];
                            if (p == "Mysterious")
                                return new string[] { "\"Interesting. Tell me more.\"", "\"That's dangerous to say to someone like me.\"" }[rng.Next(2)];
                            return warm
                                ? new string[] { "\"I... wasn't expecting that. But okay.\"", "\"That's... actually kind of mutual.\"", "\"Me too. I think.\"" }[rng.Next(3)]
                                : new string[] { "~r~\"That's a big statement.\"", "~r~\"Don't say things you don't mean.\"", "~r~\"I need time to think.\"" }[rng.Next(3)];
                    }
                    break;
                }

                // ── 5: Ask Personal Stuff ──────────────────────────────
                case 5:
                    if (rel < 10)
                        return "~r~\"You're basically a stranger. Ask me something else.\"";
                    switch (item)
                    {
                        case 0: // "What are you really like?"
                            if (d != null && (d.KnownTopics & (1L << 31)) != 0)
                            {
                                if (p == "Mysterious")   return new string[] { "~r~\"You already got a glimpse. That's all I give.\"", "~r~\"I told you. That's my answer.\"" }[rng.Next(2)];
                                if (p == "Unstable")     return new string[] { "~r~\"I gave you a version of it. That's enough.\"", "~r~\"You already asked. It's not going to change today.\"" }[rng.Next(2)];
                                if (p == "Sweet")        return new string[] { "~r~\"You already know. Nothing's changed.\"", "~r~\"I told you already. Ask me something new.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"I gave you one answer. That's it.\"", "~r~\"You already know.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You already asked. I'm still the same person.\"", "~r~\"We've been through this.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked me that.\"", "~r~\"We've been over this.\"" }[rng.Next(2)];
                            }
                            if (rel < 30) return "~r~\"I'm exactly what you see. Don't dig.\"";
                            if (p == "Mysterious") return "\"I'm still figuring that out myself.\"";
                            if (p == "Unstable")   return "\"Depends on the day. Some days even I don't know.\"";
                            if (p == "Sweet")      return "\"Probably what you'd guess. I just want people to be okay.\"";
                            return rel >= 60
                                ? "\"Honestly? I'm scared a lot. I just don't show it.\""
                                : "\"More private than I let on.\"";
                        case 1: // "Why are you out here alone?"
                            if (d != null && (d.KnownTopics & (1L << 32)) != 0)
                            {
                                if (p == "Independent")  return new string[] { "~r~\"Because I want to be. I already said that.\"", "~r~\"You already got my answer.\"" }[rng.Next(2)];
                                if (p == "嫉妒")      return new string[] { "~r~\"I told you not to get into it.\"", "~r~\"Still not something I want to discuss.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"Same reason as when you first asked.\"", "~r~\"I already told you.\"" }[rng.Next(2)];
                                if (p == "Mysterious")   return new string[] { "~r~\"You already know as much as I'm telling you.\"", "~r~\"I gave you an answer. That's your answer.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already told you.\"", "~r~\"You asked me that already.\"" }[rng.Next(2)];
                            }
                            if (rel < 30) return "~r~\"Just am. That's all.\"";
                            if (p == "Independent") return "\"Because I don't need company to have a good time.\"";
                            if (p == "嫉妒")     return "\"Let's not get into it.\"";
                            return rel >= 60 ? "\"I needed to clear my head. I have a lot going on.\"" : "\"Just needed air.\"";
                        case 2: // "What's your family like?"
                            if (d != null && (d.KnownTopics & (1L << 33)) != 0)
                            {
                                if (p == "Cold")         return new string[] { "~r~\"I told you we don't talk. End of subject.\"", "~r~\"You already have my answer on that.\"" }[rng.Next(2)];
                                if (p == "依賴")        return new string[] { "~r~\"I told you it's complicated. Please don't make me go over it again.\"", "~r~\"You already heard about my family.\"" }[rng.Next(2)];
                                if (p == "Chaotic")      return new string[] { "~r~\"You already know how that goes. Chaos. Moving on.\"", "~r~\"I told you already.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Same family I told you about. They haven't changed.\"", "~r~\"You already know the situation.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already told you about my family.\"", "~r~\"Don't make me repeat myself.\"" }[rng.Next(2)];
                            }
                            if (rel < 30) return "~r~\"We're not close enough for that.\"";
                            if (p == "Chaotic")    return "\"Exactly as chaotic as you'd expect.\"";
                            if (p == "依賴")      return "\"Complicated. But I miss them sometimes.\"";
                            if (p == "Cold")       return "\"We don't really talk.\"";
                            return rel >= 60 ? "\"It's... messy. I don't really advertise that.\"" : "\"Normal enough.\"";
                        case 3: // "What do you want out of life?"
                            if (d != null && (d.KnownTopics & (1L << 34)) != 0)
                            {
                                if (p == "Gold Digger")  return new string[] { "~r~\"Safety. I already told you. I meant it.\"", "~r~\"My priorities haven't shifted.\"" }[rng.Next(2)];
                                if (p == "Romantic")     return new string[] { "~r~\"Something real. You already know. That hasn't changed.\"", "~r~\"I told you what I want. That still stands.\"" }[rng.Next(2)];
                                if (p == "Chaotic")      return new string[] { "~r~\"I already told you I don't plan that far. Still true.\"", "~r~\"Same answer. I don't think that far ahead.\"" }[rng.Next(2)];
                                if (p == "Independent")  return new string[] { "~r~\"To not need anyone. I said that. Still feel it.\"", "~r~\"You already got my answer on that.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Same thing I said last time. Nothing's changed.\"", "~r~\"You already asked. Still figuring it out.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked me that.\"", "~r~\"I gave you my answer.\"" }[rng.Next(2)];
                            }
                            if (rel < 30) return "~r~\"Big question. Ask me when you actually know me.\"";
                            if (p == "Gold Digger")  return "\"Safety. The kind money buys.\"";
                            if (p == "Romantic")     return "\"Something real. I want to feel something real.\"";
                            if (p == "Chaotic")      return "\"Honestly? I try not to think that far ahead.\"";
                            if (p == "Independent")  return "\"To not need anyone. As sad as that sounds.\"";
                            return rel >= 60 ? "\"Less regrets. That's literally it.\"" : "\"I don't really know yet.\"";
                        case 4: // "What's your biggest regret?"
                            if (d != null && (d.KnownTopics & (1L << 35)) != 0)
                            {
                                if (p == "Cold")         return new string[] { "~r~\"You already know. I said 'caring.' Leave it.\"", "~r~\"I told you once. That's it.\"" }[rng.Next(2)];
                                if (p == "依賴")        return new string[] { "~r~\"I told you that. It was hard. Don't ask again.\"", "~r~\"That was personal. Please don't.\"" }[rng.Next(2)];
                                if (p == "Chaotic")      return new string[] { "~r~\"You already have my answer on that one.\"", "~r~\"I told you. Don't bring it up again.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You already asked. Still the same regret.\"", "~r~\"That hasn't changed since you asked.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I told you. Don't ask again.\"", "~r~\"That was hard to share. Please don't push.\"" }[rng.Next(2)];
                            }
                            if (rel < 30) return "~r~\"No.\"";
                            if (rel < 60) return "~r~\"We're not there yet.\"";
                            if (p == "依賴")      return "\"Staying too long with someone who didn't deserve it.\"";
                            if (p == "Chaotic")    return "\"Not staying long enough with someone who did.\"";
                            if (p == "Cold")       return "\"Caring.\"";
                            return "\"Something I can't change. So I try not to dwell on it.\"";
                        case 5: // "What scares you?"
                            if (d != null && (d.KnownTopics & (1L << 36)) != 0)
                            {
                                if (p == "Unstable")     return new string[] { "~r~\"You already know. Please don't bring that up again.\"", "~r~\"I shared that. It was enough. Let it be.\"" }[rng.Next(2)];
                                if (p == "嫉妒")      return new string[] { "~r~\"Being left. You already know. Don't rub it in.\"", "~r~\"I told you. Don't make it weird.\"" }[rng.Next(2)];
                                if (p == "Aggressive")   return new string[] { "~r~\"I said nothing. I meant it. Drop it.\"", "~r~\"You already got my answer on that.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"I said I'm fine. Still fine.\"", "~r~\"You already heard my answer.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Still scared of the same things. Very interesting.\"", "~r~\"You already got that answer.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already told you.\"", "~r~\"I shared that once. That's enough.\"" }[rng.Next(2)];
                            }
                            if (rel < 30) return "~r~\"That's private.\"";
                            if (p == "Unstable")   return "\"Myself, honestly. When I get in a bad place.\"";
                            if (p == "嫉妒")    return "\"Being left. I know how that sounds.\"";
                            if (p == "Aggressive") return "\"Nothing. Don't ask me that again.\"";
                            return rel >= 60 ? "\"Being alone at the end. Like really alone.\"" : "\"Things I don't control.\"";
                        case 6: // "Have you ever been in love?"
                            if (d != null && (d.KnownTopics & (1L << 37)) != 0)
                            {
                                if (p == "Romantic")     return new string[] { "~r~\"I already told you. It ended badly. Please don't make me revisit that.\"", "~r~\"You already know the story. It hurt. That's it.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"I told you. Maybe. Still don't know.\"", "~r~\"You already have my answer on that.\"" }[rng.Next(2)];
                                if (p == "嫉妒")      return new string[] { "~r~\"Once. I already said. Please don't.\"", "~r~\"I told you. It's not something I like talking about.\"" }[rng.Next(2)];
                                if (p == "依賴")        return new string[] { "~r~\"I already told you. Every time they leave. Please don't make me say it again.\"", "~r~\"You already heard that. Please.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You already asked. Still the same answer.\"", "~r~\"That conversation happened already.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already answered that.\"", "~r~\"That conversation is done.\"" }[rng.Next(2)];
                            }
                            if (rel < 30) return "~r~\"That's not a first-date question.\"";
                            if (p == "Romantic")   return "\"Yes. It ended badly. But I'd do it again.\"";
                            if (p == "Cold")       return "\"Maybe. It didn't feel the way people describe it.\"";
                            if (p == "嫉妒")    return "\"Once. I don't recommend it.\"";
                            if (p == "依賴")      return "\"I thought so. Every time. Then they leave.\"";
                            return rel >= 60 ? "\"Yeah. Once. Didn't work out.\"" : "\"I'd rather not answer that.\"";
                    }
                    break;

                // ── 6: Business ────────────────────────────────────────────────────────────
                case 6:
                    // Business branch only makes full sense in prostitution or hooker mode
                    if (sandboxMode && d != null && !d.IsHooker)
                        return "~r~\"What are you implying?\"";
                    switch (item)
                    {
                        case 0: // "What do you offer?"
                            if (p == "Gold Digger")  return "\"More than you can afford. Probably.\"";
                            if (p == "Cold")         return "\"The basics. Don't expect extras.\"";
                            if (p == "Sweet")        return "\"Whatever you need. Within reason.\"";
                            return "\"You'll find out when we get to it.\"";
                        case 1: // "Can we negotiate?"
                            if (greed >= 0.75)       return "~r~\"Price is the price.\"";
                            if (p == "Manipulative") return "\"Depends how you ask.\"";
                            if (p == "Sweet")        return "\"Maybe. You seem decent.\"";
                            return "\"I'll listen. Don't lowball me.\"";
                        case 2: // "What costs extra?"
                            if (p == "Gold Digger")  return "\"Everything memorable.\"";
                            if (p == "Cold")         return "\"Asking stupid questions.\"";
                            if (p == "Manipulative") return "\"Depends what you want me to pretend.\"";
                            return "\"We'll talk about it when it comes up.\"";
                        case 3: // "Do you do repeats?"
                            if (p == "Cold" || p == "Independent") return "\"If you're not a problem. Don't be a problem.\"";
                            if (p == "依賴")        return "\"I prefer regulars, honestly.\"";
                            return warm ? "\"Yeah. Regulars get treated better.\"" : "\"Depends.\"";
                        case 4: // "What's your rule on kissing?"
                            if (attachment >= 0.65)  return "~r~\"I don't. That's personal.\"";
                            if (p == "Gold Digger")  return "\"It costs extra.\"";
                            if (p == "Cold")         return "\"I don't.\"";
                            return warm ? "\"I'm selective about it.\"" : "\"Not usually.\"";
                        case 5: // "Discount for regulars?"
                            if (greed >= 0.80)       return "~r~\"Ha. No.\"";
                            if (p == "Sweet")        return "\"You've been good to me. Maybe.\"";
                            if (p == "Manipulative") return "\"That depends what you do for me in return.\"";
                            return rel > 20 ? "\"You've earned a little goodwill.\"" : "\"Prove you're worth it first.\"";
                        case 6: // "Can I book you again later?"
                            if (p == "Independent")  return "\"I'm around. No promises.\"";
                            if (p == "依賴")        return "\"Please do. I mean... sure.\"";
                            if (p == "Cold")         return "\"If I feel like it.\"";
                            return warm ? "\"Yeah. You know where to find me.\"" : "\"We'll see.\"";
                    }
                    break;

                // ── 2: Check Mood ──────────────────────────────────────────────────────────
                case 2:
                    string mood = (d != null && d.Mood != null && d.Mood.Length > 0) ? d.Mood : "Relaxed";
                    if (d != null) d.Mood = mood; // ensure it's initialised
                    switch (item)
                    {
                        case 0: // "How are you feeling?"
                            if (d != null && (d.KnownTopics & (1 << 12)) != 0)
                            {
                                if (mood == "Annoyed")   return new string[] { "~r~\"I said I'm not great. Stop asking.\"", "~r~\"You already know. Leave it.\"" }[rng.Next(2)];
                                if (mood == "Alert")   return new string[] { "~r~\"Still alert. Still fine.\"", "~r~\"You already heard my answer.\"" }[rng.Next(2)];
                                if (mood == "Playful")   return new string[] { "~r~\"Still good! You can stop checking.\"", "~r~\"You already asked. I'm great.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"Same as I said.\"", "~r~\"I told you already.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Same feeling I had when you asked last time.\"", "~r~\"I told you how I feel.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked me that.\"", "~r~\"I told you how I feel.\"" }[rng.Next(2)];
                            }
                            if (mood == "Annoyed")  return "~r~\"Honestly? Like I'd rather be somewhere else right now.\"";
                            if (mood == "Alert")  return "\"Careful. You can't really trust anyone out here.\"";
                            if (mood == "Playful")  return "\"Pretty good actually. You're not boring.\"";
                            if (mood == "Needy")    return "\"...Okay. I just don't want to be alone.\"";
                            if (mood == "Jealous")  return "~r~\"Fine. Why? Did something happen?\"";
                            // Fallback varies by time and weather
                            if (isRain)    return warm ? "\"A little damp but fine, honestly.\"" : "\"Cold. Wet. Could be better.\"";
                            if (isSnow)    return warm ? "\"Freezing. But kind of peaceful.\"" : "\"Like I've been standing in the cold too long.\"";
                            if (isNight)   return "\"Relaxed. It's a good night.\"";
                            return "\"Alright. Just another day.\"";
                        case 1: // "You okay?"
                            if (d != null && (d.KnownTopics & (1 << 13)) != 0)
                            {
                                if (mood == "Annoyed")   return new string[] { "~r~\"I said I'm fine. Stop checking.\"", "~r~\"You already asked. I'm handling it.\"" }[rng.Next(2)];
                                if (mood == "Alert")   return new string[] { "~r~\"Still fine. Stop looking at me like that.\"", "~r~\"You already asked.\"" }[rng.Next(2)];
                                if (mood == "Needy")     return new string[] { "~r~\"You already asked. Still better.\"", "~r~\"You don't have to keep checking on me.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"You already asked. I'm still okay.\"", "~r~\"Asked. Answered.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Yes, still okay. You already asked.\"", "~r~\"I already answered that.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"Yes, still okay. You already asked.\"", "~r~\"I already answered that.\"" }[rng.Next(2)];
                            }
                            if (mood == "Annoyed")  return "~r~\"I'm fine. Just leave it.\"";
                            if (mood == "Alert")  return "\"Why are you asking?\"";
                            if (mood == "Needy")    return "\"Better now that you're here. Is that weird?\"";
                            return warm ? "\"Yeah. I'm good, thanks.\"" : "\"I'm always okay.\"";
                        case 2: // "You seem tense."
                            if (d != null && (d.KnownTopics & (1 << 14)) != 0)
                            {
                                if (mood == "Annoyed")   return new string[] { "~r~\"You already said that. Yes. I'm tense. Moving on.\"", "~r~\"Still the same. You don't need to keep pointing it out.\"" }[rng.Next(2)];
                                if (mood == "Alert")   return new string[] { "~r~\"I told you. Alert, not tense. Stop saying that.\"", "~r~\"We went over this.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You keep saying that. Noted. Still moving on.\"", "~r~\"You really like pointing that out.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"You already mentioned that.\"", "~r~\"You keep saying that.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You keep saying that.\"", "~r~\"You already mentioned it.\"" }[rng.Next(2)];
                            }
                            if (mood == "Annoyed")  return "\"Yeah. Something came up. I'll get over it.\"";
                            if (mood == "Alert")  return "\"I'm just... aware. That's different from tense.\"";
                            if (mood == "Relaxed")  return "\"Do I? I feel fine.\"";
                            return "\"Maybe. I'll shake it off.\"";
                        case 3: // "You look happy."
                            if (d != null && (d.KnownTopics & (1 << 15)) != 0)
                            {
                                if (mood == "Playful")   return new string[] { "~r~\"Yes, you said that. I'm still happy!\"", "~r~\"You noticed the first time too.\"" }[rng.Next(2)];
                                if (mood == "Annoyed")   return new string[] { "~r~\"I told you I'm not. Why do you keep saying that?\"", "~r~\"Still not happy. Still annoyed. Stop commenting.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"We established that already.\"", "~r~\"Yes, you told me. I know.\"" }[rng.Next(2)];
                                if (p == "Dominant")     return new string[] { "~r~\"You already said that. I heard you the first time.\"", "~r~\"Already noted.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already said that.\"", "~r~\"I know, you told me.\"" }[rng.Next(2)];
                            }
                            if (mood == "Playful")  return "\"I am! Is it obvious?\"";
                            if (mood == "Annoyed")  return "~r~\"I'm not, actually. But thanks.\"";
                            if (mood == "Relaxed")  return isNight ? "\"I'm in a good place tonight.\"" : "\"I'm in a good place.\"";
                            return warm ? "\"Ha. You noticed.\"" : "\"I look the same as always.\"";
                        case 4: // "You seem distracted."
                            if (d != null && (d.KnownTopics & (1 << 16)) != 0)
                            {
                                if (mood == "Alert")   return new string[] { "~r~\"I told you. Still thinking. Still fine.\"", "~r~\"You already noticed. I'm okay.\"" }[rng.Next(2)];
                                if (mood == "Jealous")   return new string[] { "~r~\"Still the same thing on my mind. Drop it.\"", "~r~\"You already asked. I don't want to talk about it.\"" }[rng.Next(2)];
                                if (mood == "Annoyed")   return new string[] { "~r~\"Yes. You keep pointing it out. That's not helping.\"", "~r~\"I know. You already told me.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"You already said that.\"", "~r~\"I heard you the first time.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already pointed that out.\"", "~r~\"You asked me that already.\"" }[rng.Next(2)];
                            }
                            if (mood == "Alert")  return "\"Just thinking.\"";
                            if (mood == "Jealous")  return "\"Something's on my mind. Forget it.\"";
                            if (mood == "Annoyed")  return "\"Yeah. Something's bothering me but it's not worth explaining.\"";
                            return warm ? "\"Sorry. I was somewhere else for a second.\"" : "\"I'm here, aren't I?\"";
                    }
                    break;

                // ── 1: Small Talk ───────────────────────────────────────────────────────────
                case 1:
                    switch (item)
                    {
                        case 0: // "Nice weather."
                            if (d != null && (d.KnownTopics & (1 << 6)) != 0)
                            {
                                if (p == "Sarcastic")    return new string[] { "~r~\"You already talked about the weather. New material please.\"", "~r~\"Still the same sky. Yes.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"We already covered the weather.\"", "~r~\"I know what it's doing outside.\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"We already did this one! Step it up.\"", "~r~\"Ha, really? Weather again?\"" }[rng.Next(2)];
                                if (p == "Romantic")     return new string[] { "~r~\"You already said that.\"", "~r~\"We already talked about this.\"" }[rng.Next(2)];
                                return isNight
                                    ? new string[] { "~r~\"You already commented on the night.\"", "~r~\"We've been through that one.\"" }[rng.Next(2)]
                                    : new string[] { "~r~\"Yea, I can see that. Thanks.\"", "~r~\"Yikes...\"" }[rng.Next(2)];
                            }
                            if (isRain)
                            {
                                if (p == "Sarcastic")  return "\"Yeah. Real great. I'm soaked.\"";
                                if (p == "Romantic")   return "\"I actually like the rain. It's quiet.\"";
                                if (p == "Cold")       return "\"It's raining. So.\"";
                                if (p == "Playful")    return "\"Ha! 'Nice weather.' It's pouring on me right now.\"";
                                return warm ? "\"I mean... it's a little wet. But I don't mind.\"" : "\"...You know it's raining, right?\"";
                            }
                            if (isSnow)
                            {
                                if (p == "Romantic")   return "\"I love nights like this. Everything goes quiet.\"";
                                if (p == "Cold")       return "\"It's cold. Yeah.\"";
                                if (p == "Playful")    return "\"It's freezing and I love it.\"";
                                return warm ? "\"It's beautiful out, honestly.\"" : "\"Sure. If you like cold.\"";
                            }
                            if (isFog)
                            {
                                if (p == "Mysterious") return "\"I like nights like this. Hard to see who's watching.\"";
                                if (p == "Sarcastic")  return "\"Yeah, very scenic. Can't see a thing.\"";
                                return warm ? "\"It's kind of eerie, right? I like it.\"" : "\"It's foggy. Not sure 'nice' is the word.\"";
                            }
                            if (isNight && isHot)
                            {
                                if (p == "Party Girl") return "\"Perfect night out. I'll take it.\"";
                                if (p == "Romantic")   return "\"Warm nights are the best. Everything feels slower.\"";
                                return warm ? "\"It's a nice night. I'll give you that.\"" : "\"Little warm for my taste.\"";
                            }
                            if (p == "Sarcastic")   return "\"Ground-breaking observation.\"";
                            if (p == "Playful")     return "\"Ha, really? Starting with weather?\"";
                            if (p == "Cold")        return "\"It's fine.\"";
                            if (isNight)   return warm ? "\"Right? I like nights like this.\"" : "\"Sure.\"";
                            return warm ? "\"Yeah, not bad out here.\"" : "\"It's alright.\"";
                        case 1: // "You hungry?"
                            if (d != null && (d.KnownTopics & (1 << 7)) != 0)
                            {
                                if (p == "Gold Digger")  return new string[] { "~r~\"You already asked. Offer first, then ask.\"", "~r~\"Same answer as before.\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"Still hungry. Always hungry. You going to do something about it?\"", "~r~\"You already asked that!\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"I already answered that.\"", "~r~\"No. Still no.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You asked me that already. Still the same answer.\"", "~r~\"Bold follow-up.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked.\"", "~r~\"I already answered that.\"" }[rng.Next(2)];
                            }
                            if (p == "Gold Digger") return "\"Are you offering to buy?\"";
                            if (p == "Playful")     return "\"Always. What are you thinking?\"";
                            if (p == "Cold")        return "\"No.\"";
                            return warm ? "\"A little, actually. Why?\"" : "\"I'm fine.\"";
                        case 2: // "Been busy?"
                            if (d != null && (d.KnownTopics & (1 << 8)) != 0)
                            {
                                if (p == "Street Smart") return new string[] { "~r~\"I told you. Same situation.\"", "~r~\"You already got the update.\"" }[rng.Next(2)];
                                if (p == "Party Girl")   return new string[] { "~r~\"You already heard about it! Catch up.\"", "~r~\"I already told you how it's been.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"I already told you.\"", "~r~\"You know the answer.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"We literally just covered this.\"", "~r~\"Busy. Same as I said.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already told you.\"", "~r~\"We went over this.\"" }[rng.Next(2)];
                            }
                            if (p == "Street Smart") return isNight ? "\"It's been a night. Let's leave it at that.\"" : "\"It's been a day. Let's leave it at that.\"";
                            if (p == "Party Girl")   return isNight ? "\"Please yes. Just got here and I'm already tired.\"" : "\"It's barely started. Give it time.\"";
                            return warm ? "\"Not really. Kind of glad you stopped by.\"" : "\"I don't really track that.\"";
                        case 3: // "You like this city?"
                            if (d != null && (d.KnownTopics & (1 << 9)) != 0)
                            {
                                if (p == "Chaotic")      return new string[] { "~r~\"You already know how I feel about it.\"", "~r~\"Still love it. Now what?\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"It's still a city. I still said what I said.\"", "~r~\"You already have my answer.\"" }[rng.Next(2)];
                                if (p == "Romantic")     return new string[] { "~r~\"We talked about the city already.\"", "~r~\"You already know my take.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Yep. Still feel the same way as when you asked.\"", "~r~\"We covered the city. Move on.\"" }[rng.Next(2)];
                                if (p == "Classy")       return new string[] { "~r~\"My opinion on the city hasn't changed.\"", "~r~\"You already know what I think.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already know my answer.\"", "~r~\"I told you how I feel about it.\"" }[rng.Next(2)];
                            }
                            if (p == "Chaotic")    return "\"It's a disaster. I love it.\"";
                            if (p == "Classy")     return "\"It has its moments. Not always the right ones.\"";
                            if (p == "Cold")       return "\"It's a city.\"";
                            if (p == "Romantic")   return "\"Parts of it. The parts most people miss.\"";
                            return warm ? "\"Yeah. I grew up around here.\"" : "\"It's okay.\"";
                        case 4: // "You seem tired."
                            if (d != null && (d.KnownTopics & (1 << 10)) != 0)
                            {
                                if (p == "Sarcastic")    return new string[] { "~r~\"Still looks the same to you? Great. Moving on.\"", "~r~\"You already noticed that. I remember.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"Still not tired. Still don't care that you think so.\"", "~r~\"You already said that.\"" }[rng.Next(2)];
                                if (p == "依賴")        return new string[] { "~r~\"You already asked me that. I said I'm fine.\"", "~r~\"I know. You said.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already said that.\"", "~r~\"You really like repeating yourself, don't you?\"" }[rng.Next(2)];
                            }
                            if (p == "依賴")      return "\"I am. I haven't been sleeping great.\"";
                            if (p == "Cold")       return "\"I'm not. I always look like this.\"";
                            if (p == "Sarcastic")  return "\"And yet, still better-looking than you. So.\"";
                            return warm ? "\"A little. Long day.\"" : "\"I'm fine.\"";
                        case 5: // "What have you been up to?"
                            if (d != null && (d.KnownTopics & (1 << 11)) != 0)
                            {
                                if (p == "Party Girl")   return new string[] { "~r~\"Told you already. Same situation.\"", "~r~\"You already heard the story.\"" }[rng.Next(2)];
                                if (p == "Mysterious")   return new string[] { "~r~\"I gave you what I give anyone. That's all you get.\"", "~r~\"Same as last time.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Same thing I said five minutes ago.\"", "~r~\"You already have the full picture.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"Still stuff. Still not your concern.\"", "~r~\"I already answered that.\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"Ha, you already asked! Are you forgetting things?\"", "~r~\"I told you already. Keep up.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already told you.\"", "~r~\"Same as before.\"" }[rng.Next(2)];
                            }
                            if (p == "Party Girl") return "\"Just got out of a thing. Long story.\"";
                            if (p == "Mysterious") return "\"A little of this. A little of that.\"";
                            if (p == "Cold")       return "\"Things. Why?\"";
                            return warm ? "\"Nothing exciting. Just the usual.\"" : "\"Stuff.\"";
                    }
                    break;
            }
            // Fallback
            return "\"...\"";
        }

        /// <summary>
        /// Prostitution-specific approach line response table.
        /// cluster 0-4: Gentle/Confident/Cool/Flattering/Playful.
        /// lineIndex: which line within the cluster was selected.
        /// firstEncounter: true = 4-line set, false = 3-line returning set.
        /// warm: true = NPC has higher friendliness (warmer tone).
        /// accepted: true = positive response, false = rejection.
        /// </summary>

        private string[] GetProstApproachLineResponse(int cluster, int lineIndex, bool firstEncounter, bool warm, string p = "")
        {
            // Personality shorthands used throughout
            bool isDominant     = p == "Dominant";
            bool isCold         = p == "Cold";
            bool isGoldDigger   = p == "Gold Digger";
            bool isStreetSmart  = p == "Street Smart";
            bool isSarcastic    = p == "Sarcastic";
            bool isPlayful      = p == "Playful";
            bool isShy          = p == "Shy" || p == "Romantic" || p == "Sweet";
            bool isAggressive   = p == "Aggressive";
            bool isManipulative = p == "Manipulative";

            if (firstEncounter)
            {
                switch (cluster)
                {
                    // ── 0: Gentle first encounter ─────────────────────────────────────────────
                    case 0:
                        switch (lineIndex)
                        {
                            case 0: // "Hey... you working?"
                                if (isDominant)    return new string[] { "Yeah I'm working. What do you need.", "Working. What?" };
                                if (isCold)        return new string[] { "Yeah. What.", "Working. What do you want?" };
                                if (isGoldDigger)  return new string[] { "I am. What are you offering?", "Working. Show me the money first." };
                                if (isStreetSmart) return new string[] { "Yeah. You know the deal. What do you need?", "Working. Don't waste my time." };
                                if (isSarcastic)   return new string[] { "What does it look like? Yeah. What do you want.", "Obviously. What do you need?" };
                                if (isShy)         return new string[] { "Yeah... what are you looking for?", "Working. What did you have in mind?" };
                                return warm ? new string[] { "Yeah, I'm working. What do you need, hun?", "What are you looking for?" }
                                           : new string[] { "Yeah. What do you want?", "I'm working. What do you need?" };
                            case 1: // "Sorry — I don't really know how to ask this. You available?"
                                if (isDominant)    return new string[] { "Don't be sorry. Just say what you want.", "Available. What do you need." };
                                if (isCold)        return new string[] { "Yeah. Get to the point.", "Available. What?" };
                                if (isSarcastic)   return new string[] { "Ha. That's one way to ask. Yeah, I'm available.", "You found a way. What do you want?" };
                                if (isShy)         return new string[] { "It's okay. What are you looking for?", "Ha, relax. What do you need?" };
                                if (isGoldDigger)  return new string[] { "Available, yes. Let's see if you can afford it.", "I'm available. What do you have in mind?" };
                                return warm ? new string[] { "Ha, relax. I'll make it easy. What are you looking for?", "Don't worry. It's not complicated. What do you need?" }
                                           : new string[] { "I can tell. What do you want?", "It's not that hard. What do you need?" };
                            case 2: // "I'm not trying to be weird. How much for your time?"
                                if (isGoldDigger)  return new string[] { "Depends on what you want. Name the service.", "Depends on the service. Let's talk." };
                                if (isDominant)    return new string[] { "Depends what you want. What is it?", "Name it and I'll give you a number." };
                                if (isCold)        return new string[] { "Depends on the service.", "What do you want?" };
                                if (isSarcastic)   return new string[] { "A little weird but okay. Depends on the service.", "You're not weird. Depends what you want." };
                                return warm ? new string[] { "Depends what you need. What do you have in mind?", "I've got time. What are you after?" }
                                           : new string[] { "Depends on the service.", "What do you want?" };
                            case 3: // "Excuse me. Looking for company. Is that something you do?"
                                if (isSarcastic)   return new string[] { "That's the polite way to put it. Yeah. What do you want?", "Very diplomatic. Yes. What do you need?" };
                                if (isDominant)    return new string[] { "That's what I'm here for. What kind of company?", "Yeah. Tell me what you want." };
                                if (isGoldDigger)  return new string[] { "That's something I do. What do you need and what can you spend?", "Depends on your budget. What do you want?" };
                                if (isShy)         return new string[] { "Yeah... that's what I do. What are you looking for?", "Yeah. What kind of company?" };
                                return warm ? new string[] { "That's what I'm here for. What do you need?", "Yeah. What kind of company?" }
                                           : new string[] { "That's the business. What do you want?", "What kind?" };
                        }
                        break;

                    // ── 1: Confident first encounter ──────────────────────────────────────────
                    case 1:
                        switch (lineIndex)
                        {
                            case 0: // "You working? How much?"
                                if (isDominant)    return new string[] { "Working. Price depends on the service. Name it.", "Yeah. Service first." };
                                if (isGoldDigger)  return new string[] { "I am. The price might surprise you. What do you want?", "Working. And expensive. What do you need?" };
                                if (isCold)        return new string[] { "Yeah. Depends what you want.", "Working. What?" };
                                if (isStreetSmart) return new string[] { "Working. You know how this goes. What do you want?", "Yeah. Depends on the service." };
                                if (isSarcastic)   return new string[] { "Direct. Good. Working. What do you want?", "Yeah, working. Price depends. What is it?" };
                                return warm ? new string[] { "Yeah, I'm working. Depends what you need.", "What are you after?" }
                                           : new string[] { "Yeah. Depends what you want.", "I'm working. What?" };
                            case 1: // "I've got cash. What are you offering?"
                                if (isGoldDigger)  return new string[] { "Good. Cash is the right start. What do you want first?", "Show me the cash and we'll talk services." };
                                if (isDominant)    return new string[] { "Tell me what you want first. Then we talk price.", "What you want first. Price second." };
                                if (isCold)        return new string[] { "You name what you want first.", "Service first. Then cash." };
                                if (isManipulative) return new string[] { "Interesting. What do you need? I'll decide the price.", "I offer a lot. What can you afford?" };
                                return warm ? new string[] { "Good. Tell me what you want first.", "What's the service? Then we talk price." }
                                           : new string[] { "You name what you want first.", "Service first. Then cash." };
                            case 2: // "Name your rate. I'm not here to haggle."
                                if (isGoldDigger)  return new string[] { "Good attitude. I appreciate that. What do you want?", "I like that. What's the service?" };
                                if (isDominant)    return new string[] { "Smart. Depends what you want.", "Good. What do you need?" };
                                if (isSarcastic)   return new string[] { "Ha. I like that attitude. Depends on the service.", "No haggling. Respect. What do you want?" };
                                if (isStreetSmart) return new string[] { "Good. Keeps it simple. What do you want?", "That works for me. What is it?" };
                                return warm ? new string[] { "Depends on the service. What do you need?", "I appreciate that. Tell me what you want." }
                                           : new string[] { "Depends on the service.", "What do you want?" };
                            case 3: // "Let's skip the small talk. What do you charge?"
                                if (isDominant)    return new string[] { "Good. I like direct. Depends on the service.", "Service first, then price." };
                                if (isCold)        return new string[] { "Depends what you want.", "Service first." };
                                if (isGoldDigger)  return new string[] { "I charge a lot. Still interested? What do you want?", "Depends on the service. What did you have in mind?" };
                                if (isSarcastic)   return new string[] { "Finally someone who skips the small talk. What's the service?", "Thank you. Depends on what you need." };
                                return warm ? new string[] { "Depends what you're after. Name it.", "What kind of service?" }
                                           : new string[] { "Depends what you want.", "Service first." };
                        }
                        break;

                    // ── 2: Cool first encounter ───────────────────────────────────────────────
                    case 2:
                        switch (lineIndex)
                        {
                            case 0: // "You available?"
                                if (isCold)        return new string[] { "Yeah. What.", "Available. What do you want?" };
                                if (isDominant)    return new string[] { "For the right service. What do you need.", "Depends. What do you want?" };
                                if (isStreetSmart) return new string[] { "Yeah. You know the game. What do you need?", "Available. What?" };
                                if (isSarcastic)   return new string[] { "Do I look unavailable? Yeah. What do you need.", "Available. What do you want?" };
                                return warm ? new string[] { "Yeah, I'm available. What are you looking for?", "Yeah. What do you need?" }
                                           : new string[] { "Yeah. What do you want?", "What?" };
                            case 1: // "What's your rate?"
                                if (isGoldDigger)  return new string[] { "My rate? Depends on what you want. What is it?", "Good question. Depends on the service." };
                                if (isCold)        return new string[] { "Depends on the service.", "What do you want?" };
                                if (isDominant)    return new string[] { "Depends on the service. Name it.", "What do you want? Then I give you a number." };
                                return warm ? new string[] { "Depends what you need. What do you have in mind?", "I've got options. What are you after?" }
                                           : new string[] { "Depends on the service.", "What do you want?" };
                            case 2: // "You free tonight?" / "You free right now?"
                                if (isCold)        return new string[] { "I'm working. What do you need?", "What kind?" };
                                if (isPlayful)     return new string[] { "Free-ish. What did you have in mind?", "Depends on the offer. What do you need?" };
                                if (isDominant)    return new string[] { "Free enough. What do you want?", "Working. What is it?" };
                                return warm ? new string[] { "Yeah, I'm working. What kind of date?", "Working. What do you need?" }
                                           : new string[] { "I'm working. What do you need?", "What kind?" };
                            case 3: // "Got any openings?"
                                if (isGoldDigger)  return new string[] { "Depends what you're offering. What do you want?", "Could have. What do you need?" };
                                if (isSarcastic)   return new string[] { "Ha. Yeah. What do you need?", "I've got time. What is it?" };
                                if (isCold)        return new string[] { "Depends. What do you need?", "What?" };
                                return warm ? new string[] { "I've got time. What are you after?", "Yeah, I got time. Tell me what you want." }
                                           : new string[] { "Depends. What do you need?", "What?" };
                        }
                        break;

                    // ── 3: Flattering first encounter ─────────────────────────────────────────
                    case 3:
                        switch (lineIndex)
                        {
                            case 0: // "I'd be an idiot not to say something."
                                if (isDominant)    return new string[] { "Yeah, you would've. What do you need.", "Good call. What do you want?" };
                                if (isGoldDigger)  return new string[] { "Smart man. What do you want and what can you spend?", "Good instinct. What's the service?" };
                                if (isSarcastic)   return new string[] { "Ha. That's flattery. But okay. What do you need?", "I've heard better, but okay. What do you want?" };
                                if (isShy)         return new string[] { "That's... sweet. What are you looking for?", "Ha, thanks. What do you need?" };
                                return warm ? new string[] { "Ha. Flattery and business in one breath. I like it. What do you need?", "You know how to talk. What are you after?" }
                                           : new string[] { "I know. What do you want?", "Correct. What?" };
                            case 1: // "You're exactly my type."
                                if (isGoldDigger)  return new string[] { "Good taste is expensive. What do you need?", "Lucky you. What's the service?" };
                                if (isDominant)    return new string[] { "I hear that a lot. What do you want.", "Good for you. What do you need?" };
                                if (isSarcastic)   return new string[] { "Everyone says that. What do you need?", "Sure I am. What do you want?" };
                                if (isShy)         return new string[] { "Oh... that's sweet. What are you looking for?", "You're right, I... what do you need?" };
                                return warm ? new string[] { "Ha. You're sweet. What are you looking for?", "You're right, I am. What do you need?" }
                                           : new string[] { "You're right. What do you want?", "Tell me what you want." };
                            case 2: // "Don't take this the wrong way, but you're gorgeous."
                                if (isDominant)    return new string[] { "I never take it the wrong way. What do you need.", "I know I am. What do you want?" };
                                if (isCold)        return new string[] { "More than you think. What do you want?", "Depends. What?" };
                                if (isSarcastic)   return new string[] { "Bold but okay. What do you want?", "Ha. Fine. What do you need?" };
                                if (isShy)         return new string[] { "Oh... thank you. What did you have in mind?", "That's nice. What are you looking for?" };
                                return warm ? new string[] { "Ha. I like you already. What do you need?", "Good question. What are you after?" }
                                           : new string[] { "More than you think. What do you want?", "Depends. What?" };
                            case 3: // "You look like you're good at your job."
                                if (isGoldDigger)  return new string[] { "The best in the area. What do you need?", "You noticed. What are you after?" };
                                if (isDominant)    return new string[] { "I am. What do you need?", "Yeah. What's the service?" };
                                if (isSarcastic)   return new string[] { "I mean, yeah. What do you want?", "Obviously. What do you need?" };
                                if (isStreetSmart) return new string[] { "Years of practice. What do you need?", "That's the idea. What do you want?" };
                                return warm ? new string[] { "Yeah, right now is good. What are you after?", "Thanks. What do you need?" }
                                           : new string[] { "Yeah. What do you want?", "What do you need?" };
                        }
                        break;

                    // ── 4: Playful first encounter ────────────────────────────────────────────
                    case 4:
                        switch (lineIndex)
                        {
                            case 0: // "Ha, I like how you asked that."
                                if (isPlayful)     return new string[] { "Ha! I like you already. What do you need?", "Direct and fun. I like it. What do you want?" };
                                if (isDominant)    return new string[] { "Cute. What do you want.", "Fine. What do you need?" };
                                if (isCold)        return new string[] { "Depends what you're after.", "What do you want?" };
                                if (isSarcastic)   return new string[] { "Ha. Points for style. What do you need?", "Bold. What do you want?" };
                                return warm ? new string[] { "Ha, I like how you asked that. What do you want?", "Direct. I respect it. What do you need?" }
                                           : new string[] { "Depends what you're after.", "What do you want?" };
                            case 1: // "You busy or can I steal you for an hour?"
                                if (isPlayful)     return new string[] { "Ha! Maybe. What are you after?", "Steal me? I'm intrigued. What do you need?" };
                                if (isGoldDigger)  return new string[] { "Depends what 'steal me' means financially. What do you want?", "Maybe. What are you offering?" };
                                if (isCold)        return new string[] { "Maybe. What do you want?", "Depends." };
                                return warm ? new string[] { "Ha. Maybe I am. What do you need?", "I might be free. What are you after?" }
                                           : new string[] { "Maybe. What do you want?", "Depends." };
                            case 2: // "You look bored. I can fix that."
                                if (isPlayful)     return new string[] { "Ha! Can you now. What do you have in mind?", "Big words. What are you offering?" };
                                if (isDominant)    return new string[] { "Can you? Let's see. What do you need.", "Bored? Maybe. What do you want?" };
                                if (isSarcastic)   return new string[] { "Ha. Big claim. What do you want?", "You think? What do you have in mind?" };
                                return warm ? new string[] { "Ha, then we're already talking. What is it?", "Good. Then tell me what you want." }
                                           : new string[] { "Tell me what it is.", "What do you want?" };
                            case 3: // "How much for a really good time?"
                                if (isGoldDigger)  return new string[] { "Ha. A 'really good time' costs accordingly. What do you want?", "Good times are expensive. What are you after?" };
                                if (isPlayful)     return new string[] { "Ha! Then let's talk money. Come on.", "Ha. I like the energy. What do you want?" };
                                if (isDominant)    return new string[] { "Ha. Depends what you mean. What do you need.", "Depends on the service. What?" };
                                if (isSarcastic)   return new string[] { "Ha. Specific ask. Depends on what you want.", "I like that framing. What do you need?" };
                                return warm ? new string[] { "Ha! Then let's talk money. Come on.", "Ha. You're paying right. What do you want?" }
                                           : new string[] { "Then let's talk money. What?", "Good. What do you want?" };
                        }
                        break;
                }
            }
            else // returning encounter
            {
                switch (cluster)
                {
                    // ── 0: Gentle returning ───────────────────────────────────────────────────
                    case 0:
                        switch (lineIndex)
                        {
                            case 0: // "Hey... you remember me?"
                                if (isDominant)    return new string[] { "Yeah, I remember. What do you want.", "I remember. What is it?" };
                                if (isCold)        return new string[] { "I know. Come on.", "Yeah. What do you want?" };
                                if (isShy)         return new string[] { "Oh, hey. Yeah I do. What do you need?", "I remember. Good to see you." };
                                if (isSarcastic)   return new string[] { "Ha. Yeah I do. Come on.", "Still here. What do you want?" };
                                return warm ? new string[] { "I remember. Good to see you again.", "Hey. Yeah, I remember. What do you need?" }
                                           : new string[] { "I know. Come on.", "Yeah. What do you want?" };
                            case 1: // "Good to see you. You free right now?"
                                if (isGoldDigger)  return new string[] { "For the right rate. What do you want?", "Yeah. What are you after?" };
                                if (isDominant)    return new string[] { "Free enough. What do you need.", "What do you want?" };
                                if (isShy)         return new string[] { "Yeah, I'm free. What do you need?", "For you? Yeah. What are you after?" };
                                return warm ? new string[] { "For you? Yeah. What do you need?", "Yeah, I'm free. What are you after?" }
                                           : new string[] { "Yeah. What do you want?", "What?" };
                            case 2: // "I was hoping I'd find you here."
                                if (isGoldDigger)  return new string[] { "Worth the search. What do you need?", "Smart man. What are you after?" };
                                if (isSarcastic)   return new string[] { "Ha. Here I am. What do you want?", "And here you are. What do you need?" };
                                if (isShy)         return new string[] { "Really? That's... nice. What do you need?", "And here I am. What are you after?" };
                                return warm ? new string[] { "And here I am. What do you need?", "Good. What are you after?" }
                                           : new string[] { "And here I am. What?", "What do you need?" };
                        }
                        break;

                    // ── 1: Confident returning ────────────────────────────────────────────────
                    case 1:
                        switch (lineIndex)
                        {
                            case 0: // "Back again. Same deal as last time?"
                                if (isDominant)    return new string[] { "Yeah. Same deal. What do you need.", "Same deal. Come on." };
                                if (isGoldDigger)  return new string[] { "Same deal means same price. You know that. Come on.", "Yeah. Same rate. What do you want?" };
                                if (isSarcastic)   return new string[] { "Ha. You're a regular now. Yeah. Come on.", "Same deal. Still standing. Come on." };
                                return warm ? new string[] { "Yeah, same deal. Come on.", "I can do that. Let's go." }
                                           : new string[] { "Yeah. Come on.", "Same deal. Let's go." };
                            case 1: // "You free right now? I've got money."
                                if (isGoldDigger)  return new string[] { "Good. Then let's get to it. What do you want?", "Cash first, then we talk. What do you want?" };
                                if (isDominant)    return new string[] { "Good. What do you need.", "Money talks. What?" };
                                if (isCold)        return new string[] { "Good. Come on.", "Then let's go." };
                                return warm ? new string[] { "Good. Show me. Come on.", "Then let's not waste it. Come on." }
                                           : new string[] { "Good. Come on.", "Then let's go." };
                            case 2: // "Let's not waste time. What's it gonna be?"
                                if (isDominant)    return new string[] { "Same as always. What do you want.", "What do you need? Tell me." };
                                if (isGoldDigger)  return new string[] { "Up to you and your wallet. What do you need?", "Same as before or something new? What?" };
                                if (isSarcastic)   return new string[] { "Ha. I like you. What do you need?", "No time wasted. What do you want?" };
                                return warm ? new string[] { "That's up to you, baby. What do you need?", "Same as always, or something different?" }
                                           : new string[] { "Tell me.", "What do you want?" };
                        }
                        break;

                    // ── 2: Cool returning ─────────────────────────────────────────────────────
                    case 2:
                        switch (lineIndex)
                        {
                            case 0: // "You again."
                                if (isDominant)    return new string[] { "Yeah, me again. What do you want.", "Me again. What do you need." };
                                if (isSarcastic)   return new string[] { "Ha. Yeah, me again. What do you need?", "Back again. What do you want?" };
                                if (isCold)        return new string[] { "You again. Come on.", "What do you want?" };
                                return warm ? new string[] { "You again. Yeah. What do you need?", "Still here. What do you want?" }
                                           : new string[] { "You again. Come on.", "What do you want?" };
                            case 1: // "Same spot as always."
                                if (isStreetSmart) return new string[] { "Consistency is smart. What do you want?", "I don't move around much. What do you need?" };
                                if (isSarcastic)   return new string[] { "Ha. Yeah same spot. What do you need?", "Always here. What do you want?" };
                                if (isGoldDigger)  return new string[] { "You know where to find me. What do you need?", "You've done your research. What?" };
                                return warm ? new string[] { "Still here. What do you need?", "Yeah, same spot. What are you after?" }
                                           : new string[] { "Yeah. What?", "What do you need?" };
                            case 2: // "Good instinct coming back."
                                if (isGoldDigger)  return new string[] { "Smart. I am worth coming back to. What do you need?", "Yeah it was. What do you want?" };
                                if (isDominant)    return new string[] { "Good. What do you need.", "Smart. What?" };
                                if (isSarcastic)   return new string[] { "Ha. Yeah, great instincts. What do you need?", "Very wise. What do you want?" };
                                return warm ? new string[] { "Smart. What do you need?", "Good instinct. What do you want?" }
                                           : new string[] { "Good. What do you want?", "What?" };
                        }
                        break;

                    // ── 3: Flattering returning ───────────────────────────────────────────────
                    case 3:
                        switch (lineIndex)
                        {
                            case 0: // "You say that every time. I appreciate it."
                                if (isGoldDigger)  return new string[] { "Consistent. I appreciate that. What do you need?", "Ha. Keep saying it. What do you want?" };
                                if (isDominant)    return new string[] { "I know you mean it. What do you want.", "Good. What do you need?" };
                                if (isSarcastic)   return new string[] { "Ha. You do say it a lot. Come on. What do you need?", "Flattery works on me. What do you want?" };
                                return warm ? new string[] { "Ha. You say that every time. I appreciate it. What do you need?", "Ha. Come on. What do you want?" }
                                           : new string[] { "You remembered right. Come on.", "What do you want?" };
                            case 1: // "You look even better than I remembered."
                                if (isGoldDigger)  return new string[] { "Good memory and good taste. What do you want?", "Ha. High praise. What do you need?" };
                                if (isDominant)    return new string[] { "I know. What do you need.", "Good eye. What?" };
                                if (isSarcastic)   return new string[] { "Ha. I do, don't I. What do you want?", "Ha. Come on. What do you need?" };
                                if (isShy)         return new string[] { "That's really sweet. What do you need?", "Aw. What are you after?" };
                                return warm ? new string[] { "And here you are. What do you need?", "I can tell. What do you want?" }
                                           : new string[] { "And here you are. What?", "Good. Come on." };
                            case 2: // "You're coming with me, right?"
                                if (isDominant)    return new string[] { "Ha. Yeah, I'm coming. What do you want.", "Bold. Yes. Come on." };
                                if (isSarcastic)   return new string[] { "Ha! Sure. Let's go. What do you need?", "Obviously. Come on." };
                                if (isCold)        return new string[] { "Obviously. What do you need?", "Correct. What?" };
                                return warm ? new string[] { "Ha. You're right. What do you need?", "Come on then. What do you want?" }
                                           : new string[] { "Obviously. What do you need?", "Correct. What?" };
                        }
                        break;

                    // ── 4: Playful returning ──────────────────────────────────────────────────
                    case 4:
                        switch (lineIndex)
                        {
                            case 0: // "Ha. Maybe. Come on."
                                if (isPlayful)     return new string[] { "Ha! A little bit. Come on.", "Ha. Don't let it go to your head. Come on." };
                                if (isDominant)    return new string[] { "Maybe. What do you want.", "Little bit. Come on." };
                                if (isSarcastic)   return new string[] { "Ha. A little bit. Come on.", "Maybe. Let's go." };
                                return warm ? new string[] { "Ha. Maybe. Come on.", "A little bit. Don't let it go to your head." }
                                           : new string[] { "Little bit. Come on.", "Maybe. What?" };
                            case 1: // "Hi. Ha. Predictable's fine with me."
                                if (isPlayful)     return new string[] { "Ha! I like predictable. Come on.", "Ha. Reliable, not predictable. Come on." };
                                if (isGoldDigger)  return new string[] { "Predictable means money. Hi. Come on.", "Hi. Consistent business. What do you want?" };
                                if (isSarcastic)   return new string[] { "Ha. Very predictable. Yeah. Come on.", "Hi. Predictable pays. Come on." };
                                return warm ? new string[] { "Hi. Yeah, I'm free. What do you need?", "Ha. Predictable's fine with me. Come on." }
                                           : new string[] { "Hi. Yeah. What do you want?", "Hi. Come on." };
                            case 2: // "Ha. Already? Come on then."
                                if (isPlayful)     return new string[] { "Ha! Always ready. Come on.", "Ha. Already. Come on then." };
                                if (isGoldDigger)  return new string[] { "Let's talk money first. Come on.", "Cash first. Then we go." };
                                if (isDominant)    return new string[] { "Yeah, already. Let's go.", "Ha. Come on." };
                                return warm ? new string[] { "Ha. Already? Come on then.", "Let's talk money first. Come on." }
                                           : new string[] { "Let's talk money first.", "Yeah. Let's go." };
                        }
                        break;
                }
            }
            // Fallback
            return new string[] { "Yeah. What do you need?" };
        }

        /// <summary>
        /// Return 1-2 NPC response options for the given approach line (Casual A-Life).
        /// cluster 0-4: Gentle/Confident/Cool/Flattering/Playful (player's chosen approach style).
        /// lineIndex: which line within that cluster was selected.
        /// firstEncounter: true = 4-line set, false = 3-line returning set.
        /// warm: true = NPC has Friendliness > 0.5 (softer tone), false = cooler/sharper tone.
        /// accepted: true = positive response, false = rejection.
        /// </summary>
        private string[] GetApproachLineResponseOptions(int cluster, int lineIndex, bool firstEncounter, bool warm, bool accepted)
        {
            if (firstEncounter)
            {
                switch (cluster)
                {
                    case 0: // Gentle
                        switch (lineIndex)
                        {
                            case 0: // "Excuse me, I hope I'm not bothering you."
                                if (accepted) return warm
                                    ? new string[] { "Not at all, how can I help you?", "Of course not. What's on your mind?" }
                                    : new string[] { "You're fine. What is it?", "Go ahead, make it quick." };
                                return warm
                                    ? new string[] { "Oh, you're sweet, but now's not a good time.", "I'm a bit busy right now. Sorry." }
                                    : new string[] { "Well... you kind of are, yeah.", "Actually, a little. Move on." };
                            case 1: // "You have a really warm smile."
                                if (accepted) return warm
                                    ? new string[] { "Aw, thank you! That really made my day.", "You just made me smile even harder." }
                                    : new string[] { "That's nice of you. What do you want?", "...Thanks. Is there something I can do for you?" };
                                return warm
                                    ? new string[] { "You're kind, but I'm keeping to myself today.", "That's sweet, but I'm not looking for company." }
                                    : new string[] { "I get that a lot. Goodbye.", "Thanks. And?" };
                            case 2: // "I couldn't walk past without saying hi."
                                if (accepted) return warm
                                    ? new string[] { "I'm glad you didn't! Hi back.", "Well hi! What's up?" }
                                    : new string[] { "You said it. Now what?", "Hi. So what do you actually want?" };
                                return warm
                                    ? new string[] { "That's sweet. But please don't take it as an invitation.", "Hi. But I really need to be alone right now." }
                                    : new string[] { "You could've, though. Goodbye.", "Well you should've. Bye." };
                            case 3: // "Mind if I keep you company for a bit?"
                                if (accepted) return warm
                                    ? new string[] { "I'd like that, actually.", "Sure, why not? I could use the company." }
                                    : new string[] { "...Fine. For a bit.", "I suppose. Don't be annoying." };
                                return warm
                                    ? new string[] { "I appreciate the offer, but I'd like some quiet.", "That's kind of you. I just need some space." }
                                    : new string[] { "Actually, yes. I do mind.", "I prefer to be alone. Move on." };
                        }
                        break;
                    case 1: // Confident
                        switch (lineIndex)
                        {
                            case 0: // "Excuse me. You caught my attention."
                                if (accepted) return warm
                                    ? new string[] { "Did I? Well, you've got mine too.", "Good. Come on over." }
                                    : new string[] { "Alright. You've got about a minute.", "Fine. I'm listening." };
                                return warm
                                    ? new string[] { "Flattered. But not interested, sorry.", "Good for you. I'm still busy." }
                                    : new string[] { "Most people do. That doesn't mean anything.", "Keep walking." };
                            case 1: // "I'm not going to pretend I didn't notice you."
                                if (accepted) return warm
                                    ? new string[] { "Good, because I noticed you too.", "Honesty. I like that. Come here." }
                                    : new string[] { "Fair enough. What do you want?", "Points for directness. Go ahead." };
                                return warm
                                    ? new string[] { "I appreciate the honesty. Still not interested.", "Bold. I respect it. But still no." }
                                    : new string[] { "Smart move. Keep walking.", "At least you're honest. Still not interested." };
                            case 2: // "You look like you know what you want."
                                if (accepted) return warm
                                    ? new string[] { "I do. And right now I'm curious about you.", "Ha, you have no idea. Let's go." }
                                    : new string[] { "I do. So let's find out if you do too.", "I do. You've got one chance to impress me." };
                                return warm
                                    ? new string[] { "I do. And right now I want some space.", "True. And what I want is to be left alone. Sorry." }
                                    : new string[] { "I know exactly what I want. And it's not this.", "I do. And it isn't you, stranger." };
                            case 3: // "Got a minute?"
                                if (accepted) return warm
                                    ? new string[] { "Sure. What's going on?", "For you? Yeah, I think so." }
                                    : new string[] { "Barely. Make it count.", "One minute. Go." };
                                return warm
                                    ? new string[] { "I really don't, sorry.", "Not right now. Maybe later?" }
                                    : new string[] { "No.", "I don't. Goodbye." };
                        }
                        break;
                    case 2: // Cool
                        switch (lineIndex)
                        {
                            case 0: // "You look like you have better things to do. Same."
                                if (accepted) return warm
                                    ? new string[] { "Ha. Then why'd you stop?", "Fair point. But here we are." }
                                    : new string[] { "Then why are you talking to me?", "Yet here you are. What do you want?" };
                                return warm
                                    ? new string[] { "Then go do them.", "Right. So do I, actually." }
                                    : new string[] { "Then stop wasting mine. Goodbye.", "So go do them." };
                            case 1: // "Interesting. You're hard to read."
                                if (accepted) return warm
                                    ? new string[] { "Most people say that. You seem like you might figure it out.", "Good observation. Most give up. Come on." }
                                    : new string[] { "That's intentional. You get one chance.", "Good. Then you'll have to pay attention." };
                                return warm
                                    ? new string[] { "Good. I'll stay that way then.", "I'll take that. Goodbye." }
                                    : new string[] { "Intended to be. Now move on.", "You don't need to read me. Bye." };
                            case 2: // "I'll keep it short: you're stunning."
                                if (accepted) return warm
                                    ? new string[] { "Short and effective. I appreciate that.", "You're not bad yourself. Come on." }
                                    : new string[] { "I've heard worse. Go on.", "That works. For now." };
                                return warm
                                    ? new string[] { "Thank you. Short but still no.", "Appreciated. Still keeping to myself." }
                                    : new string[] { "Good. Keep it short. Goodbye.", "Pretty words. I'm still leaving." };
                            case 3: // "Not many people carry themselves like that."
                                if (accepted) return warm
                                    ? new string[] { "Ha, you noticed. I like you already.", "Good eyes. Come on then." }
                                    : new string[] { "Few people do. You might be worth a minute.", "That's... not a bad line. I'll give you that." };
                                return warm
                                    ? new string[] { "Thank you. I like to keep to myself though.", "That's kind. Still leaving." }
                                    : new string[] { "I know. Goodbye.", "I'm aware. Goodbye." };
                        }
                        break;
                    case 3: // Flattering
                        switch (lineIndex)
                        {
                            case 0: // "I'd be an idiot not to say something."
                                if (accepted) return warm
                                    ? new string[] { "Ha! I like that. Come on then.", "Smart man. Let's talk." }
                                    : new string[] { "That depends on what you say next. Go ahead.", "Don't prove me wrong." };
                                return warm
                                    ? new string[] { "Points for bravery. Still no thanks.", "I appreciate the effort. Not interested." }
                                    : new string[] { "Then be an idiot. I'm busy.", "The outcome is the same either way." };
                            case 1: // "You're exactly my type."
                                if (accepted) return warm
                                    ? new string[] { "Well that's convenient, because I don't hate your face either.", "Is that so? Come here and let's find out." }
                                    : new string[] { "That's what they all say. Convince me.", "I'll be the judge of that." };
                                return warm
                                    ? new string[] { "That's sweet. Types don't always work out though.", "Flattered. Still not interested." }
                                    : new string[] { "Not everyone's available just because they're your type.", "Good for you. I'm not looking." };
                            case 2: // "Don't take this the wrong way, but you're gorgeous."
                                if (accepted) return warm
                                    ? new string[] { "I'll take it exactly the right way. Thank you.", "Ha, I never take that the wrong way. Come on." }
                                    : new string[] { "I won't. What do you want?", "I rarely do. Go on." };
                                return warm
                                    ? new string[] { "Aw, that's kind. Still not going anywhere with you.", "Thank you genuinely. But no." }
                                    : new string[] { "Noted. Still not interested.", "恭維被拒絕。" };
                            case 3: // "You deserve someone paying attention."
                                if (accepted) return warm
                                    ? new string[] { "Damn right I do. And that's you, right now.", "You're not wrong. Let's see where this goes." }
                                    : new string[] { "I do. So start talking.", "Prove it then." };
                                return warm
                                    ? new string[] { "That's really sweet. I'm just not looking.", "I appreciate that. I'm fine right now." }
                                    : new string[] { "I get plenty of attention. Keep moving.", "I know. Still not interested in yours." };
                        }
                        break;
                    case 4: // Playful
                        switch (lineIndex)
                        {
                            case 0: // "Hey, I bet you get this a lot, but damn."
                                if (accepted) return warm
                                    ? new string[] { "Ha! Honestly, you're not wrong. Come on.", "I do get it a lot. But I liked how you said it." }
                                    : new string[] { "You'd be right. Let's see if you back it up.", "I do. But I'm not bored of it. Go on." };
                                return warm
                                    ? new string[] { "Ha, you're right, I do. Thanks though!", "I do. And I'm still busy. Funny how that works." }
                                    : new string[] { "I do. And I'm still not here for it. Shocker.", "You'd be surprised how often that doesn't work." };
                            case 1: // "You look like trouble. I like that."
                                if (accepted) return warm
                                    ? new string[] { "Ha! You have no idea. Come on then.", "I've been called worse. Let's see what you've got." }
                                    : new string[] { "You haven't seen anything yet.", "Good. Then we'll get along fine." };
                                return warm
                                    ? new string[] { "Ha! Maybe I am. That's exactly why you should go.", "Trouble doesn't mean available. Take care." }
                                    : new string[] { "Good instincts. Now act on them and walk away.", "Right. And trouble doesn't come easy." };
                            case 2: // "Tell me you're not as fun as you look."
                                if (accepted) return warm
                                    ? new string[] { "Oh, I'm more fun than I look. You in?", "I absolutely am. Come find out." }
                                    : new string[] { "That's something you'll have to earn.", "Bold assumption. Come on then." };
                                return warm
                                    ? new string[] { "Ha! I might be. Just not for you right now.", "I am fun. Just not with strangers today." }
                                    : new string[] { "You'll never know. Move along.", "That curiosity's not paying off today." };
                            case 3: // "Can I steal five minutes of your time?"
                                if (accepted) return warm
                                    ? new string[] { "Ha, five minutes? Sure, let's see if you're worth it.", "Fine! Five minutes. Starting now." }
                                    : new string[] { "You can try. Clock's ticking.", "Five minutes. That's all you get." };
                                return warm
                                    ? new string[] { "Oh, I wish I could. Sorry!", "I'd give them if I could. Not right now." }
                                    : new string[] { "They're not for sale. Goodbye.", "Not today. Move on." };
                        }
                        break;
                }
            }
            else // returning encounter
            {
                switch (cluster)
                {
                    case 0: // Gentle returning
                        switch (lineIndex)
                        {
                            case 0: // "Hey [name], good to see you again."
                                if (accepted) return warm
                                    ? new string[] { "Hey you! Same. Come on.", "You again! I was hoping." }
                                    : new string[] { "Hey. What do you need?", "Yeah. Hi. What's up?" };
                                return warm
                                    ? new string[] { "Oh hey... I'm not really feeling social right now. Sorry.", "Hi. I just can't right now. Maybe later?" }
                                    : new string[] { "It's fine. I'd rather be alone.", "Hi. Keep moving." };
                            case 1: // "I kept thinking about you. Want to hang out?"
                                if (accepted) return warm
                                    ? new string[] { "Yeah? That's sweet. Yes, come on.", "Ha, well I'm right here. Let's go." }
                                    : new string[] { "Is that so? Fine. Come on.", "Sure. Don't make it weird." };
                                return warm
                                    ? new string[] { "Aw... I've been busy. Maybe another time?", "That's sweet but now's not great." }
                                    : new string[] { "That's your problem. I'm busy.", "Then you've been wasting time. Move on." };
                            case 2: // "Come with me."
                                if (accepted) return warm
                                    ? new string[] { "Okay! Let's go.", "Right now? Alright, fine." }
                                    : new string[] { "...Fine.", "Where are we going?" };
                                return warm
                                    ? new string[] { "I can't right now.", "Not now." }
                                    : new string[] { "No.", "I don't think so." };
                        }
                        break;
                    case 1: // Confident returning
                        switch (lineIndex)
                        {
                            case 0: // "Hey [name]. Let's not waste time."
                                if (accepted) return warm
                                    ? new string[] { "Ha. I like that. Let's go.", "Good. Come on." }
                                    : new string[] { "Then don't. Let's go.", "Agreed. Move." };
                                return warm
                                    ? new string[] { "I'll keep it quick: no. Sorry.", "Appreciate the directness. Not today." }
                                    : new string[] { "Then stop talking. Goodbye.", "Good. Then go." };
                            case 1: // "You free?"
                                if (accepted) return warm
                                    ? new string[] { "For you? Yeah.", "Could be. What've you got in mind?" }
                                    : new string[] { "Could be. What do you want?", "For a bit. What?" };
                                return warm
                                    ? new string[] { "Not right now, sorry.", "I wish. Not today." }
                                    : new string[] { "No.", "Not for this." };
                            case 2: // "Come with me."
                                if (accepted) return warm
                                    ? new string[] { "Fine. Where to?", "Alright. Lead the way." }
                                    : new string[] { "Don't push it. Let's go.", "Maybe. Move." };
                                return warm
                                    ? new string[] { "I can't right now.", "Not now." }
                                    : new string[] { "No.", "I'm staying." };
                        }
                        break;
                    case 2: // Cool returning
                        switch (lineIndex)
                        {
                            case 0: // "[name]. Good."
                                if (accepted) return warm
                                    ? new string[] { "Ha. You remembered. Points for that.", "Hey. Good yourself. Let's go." }
                                    : new string[] { "Still here, yeah. What do you want?", "I exist. State your business." };
                                return warm
                                    ? new string[] { "Hey. Not now though.", "Hi. I need some space today." }
                                    : new string[] { "Still not interested.", "Good nothing. Bye." };
                            case 1: // "Still as hard to read as ever."
                                if (accepted) return warm
                                    ? new string[] { "And you're still trying. I like that.", "Ha. Some things don't change. Come on." }
                                    : new string[] { "That's a feature. Go on.", "You've learned nothing. Try again." };
                                return warm
                                    ? new string[] { "I'll take that as a compliment. Goodbye.", "Some things stay the same. Like my answer." }
                                    : new string[] { "That's the idea. Goodbye.", "I'll stay mysterious about that too. Bye." };
                            case 2: // "Don't make me ask twice."
                                if (accepted) return warm
                                    ? new string[] { "Ha! Bold as ever. Fine, let's go.", "You really are something. Alright." }
                                    : new string[] { "You already did. Let's go.", "That's rich. Come on then." };
                                return warm
                                    ? new string[] { "Then don't. Bye.", "I appreciate it. Still no." }
                                    : new string[] { "I won't give you the chance. Goodbye.", "You asked once too many. Bye." };
                        }
                        break;
                    case 3: // Flattering returning
                        switch (lineIndex)
                        {
                            case 0: // "Hey [name], I've been thinking about you."
                                if (accepted) return warm
                                    ? new string[] { "Yeah? I'm glad. Come on.", "Ha, me too honestly. Let's go." }
                                    : new string[] { "Have you? Interesting. Come on.", "Sure you have. What do you want?" };
                                return warm
                                    ? new string[] { "Aw... I'm flattered. Not today though.", "That's sweet. I've just been... busy." }
                                    : new string[] { "Keep those thoughts to yourself.", "That's your time to manage. Not mine." };
                            case 1: // "You look even better than I remembered."
                                if (accepted) return warm
                                    ? new string[] { "Ha! I'll take it. You're not so bad yourself.", "You flatter me. Fine, come on." }
                                    : new string[] { "Good memory. Come on.", "Is that your best? Let's go." };
                                return warm
                                    ? new string[] { "You're too kind. Still can't right now.", "That's very sweet. Not today." }
                                    : new string[] { "And I remember you being more subtle. Bye.", "High standards. Still a no. Bye." };
                            case 2: // "You're coming with me, right?"
                                if (accepted) return warm
                                    ? new string[] { "...Yeah, alright. Let's go.", "Ha! That's bold. Sure." }
                                    : new string[] { "Don't push it. Let's go.", "Maybe I am. Move." };
                                return warm
                                    ? new string[] { "Not this time.", "I'm staying right here, thanks." }
                                    : new string[] { "No. I'm not.", "Wrong guess." };
                        }
                        break;
                    case 4: // Playful returning
                        switch (lineIndex)
                        {
                            case 0: // "Hey [name], ready to cause some trouble?"
                                if (accepted) return warm
                                    ? new string[] { "Always. What are we doing?", "Ha! You had me at trouble. Let's go." }
                                    : new string[] { "I was born ready. Keep up.", "Finally. Let's go." };
                                return warm
                                    ? new string[] { "Ha! As much as I'd love to... not right now.", "The spirit is willing. The timing's off." }
                                    : new string[] { "Not with you. Not today.", "Trouble? Sure. With you? No." };
                            case 1: // "Miss me?"
                                if (accepted) return warm
                                    ? new string[] { "Ha! Maybe a little. Come on.", "I hate that I did. Let's go." }
                                    : new string[] { "Don't flatter yourself. Let's go anyway.", "Sure. What do you want?" };
                                return warm
                                    ? new string[] { "A little. But still no.", "I can miss you and still need space." }
                                    : new string[] { "No.", "Not even a little. Bye." };
                            case 2: // "Come on, let's go."
                                if (accepted) return warm
                                    ? new string[] { "Alright, alright. Let's go.", "Ha, okay then." }
                                    : new string[] { "Don't rush me. Fine, let's go.", "Fine." };
                                return warm
                                    ? new string[] { "Not right now.", "I'm good here." }
                                    : new string[] { "You go. I'm staying.", "No." };
                        }
                        break;
                }
            }
            // Fallback
            return accepted
                ? new string[] { "Sure." }
                : new string[] { "Not interested." };
        }

        /// <summary>Return NPC response for Intimacy sub-branch dialogue (Ask Preferences or Test Waters).</summary>
        private string GetIntimacySubResponse(int branch, int item, ALifePedData d)
        {
            string p = (d != null && d.Personality != null) ? d.Personality : "";
            PersonalityProfile prof = (d != null) ? GetProfile(d.Personality) : null;
            double confidence   = (prof != null) ? prof.Confidence   : 0.50;
            double riskiness    = (prof != null) ? prof.Riskiness    : 0.50;
            double attachment   = (prof != null) ? prof.Attachment   : 0.50;
            int rel = (d != null) ? d.Reputation : 0;
            bool warm = IsWarmPersonality(d);

            if (branch == 0) // Ask Preferences
            {
                switch (item)
                {
                    case 0: // "Do you kiss?"
                        if (d != null && (d.KnownTopics & (1 << 25)) != 0)
                        {
                            if (p == "Cold")         return new string[] { "~r~\"I said no. Still no.\"", "~r~\"We went over this already.\"" }[rng.Next(2)];
                            if (p == "Romantic")     return new string[] { "~r~\"You already asked. My answer hasn't changed.\"", "~r~\"I told you already.\"" }[rng.Next(2)];
                            if (p == "Playful")      return new string[] { "~r~\"You already asked that. Still maybe.\"", "~r~\"I already told you. Check back later.\"" }[rng.Next(2)];
                            if (p == "Sarcastic")    return new string[] { "~r~\"You already asked. My mood answer stands.\"", "~r~\"Still the same answer.\"" }[rng.Next(2)];
                            return new string[] { "~r~\"I already told you.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                        }
                        if (attachment >= 0.65) return "~r~\"Not with just anyone.\"";
                        if (p == "Playful")     return "\"Depends on my mood. Right now? Maybe.\"";
                        if (p == "Cold")        return "~r~\"No.\"";
                        if (p == "Romantic")    return "\"If it feels right, yes.\"";
                        return warm ? "\"Sometimes. I'm picky about it.\"" : "~r~\"Not a default.\"";
                    case 1: // "Public or private?"
                        if (d != null && (d.KnownTopics & (1 << 27)) != 0)
                        {
                            if (p == "Party Girl")   return new string[] { "~r~\"You already asked. Depends on the night. Still.\"", "~r~\"Same answer. You already know.\"" }[rng.Next(2)];
                            if (p == "Classy")       return new string[] { "~r~\"Private. I already said that. Don't ask again.\"", "~r~\"My answer is the same. Private.\"" }[rng.Next(2)];
                            if (p == "Chaotic")      return new string[] { "~r~\"Either. You already knew that.\"", "~r~\"Still doesn't matter to me. You asked.\"" }[rng.Next(2)];
                            if (p == "Cold")         return new string[] { "~r~\"Private. I said it once.\"", "~r~\"You already know my answer.\"" }[rng.Next(2)];
                            return new string[] { "~r~\"You already know my answer.\"", "~r~\"I told you already.\"" }[rng.Next(2)];
                        }
                        if (riskiness < 0.30)   return "~r~\"Private. Obviously.\"";
                        if (p == "Party Girl")  return "\"Depends how late it is.\"";
                        if (p == "Chaotic")     return "\"I've done both. I'm not picky.\"";
                        if (p == "Classy")      return "\"Private. I have a reputation.\"";
                        return warm ? "\"I'm flexible, honestly.\"" : "~r~\"Does it look like I do things in public?\"";
                    case 2: // "What are you into?"
                        if (d != null && (d.KnownTopics & (1 << 24)) != 0)
                        {
                            if (p == "Dominant")    return new string[] { "~r~\"I told you what I'm into. Don't make me repeat myself.\"", "~r~\"You already know.\"" }[rng.Next(2)];
                            if (p == "Cold")        return new string[] { "~r~\"You already know. Drop it.\"", "~r~\"I told you once.\"" }[rng.Next(2)];
                            if (p == "Sarcastic")   return new string[] { "~r~\"Same list as last time. Try to retain information.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                            if (p == "Shy")         return new string[] { "~r~\"I... you already asked me that.\"", "~r~\"I told you already.\"" }[rng.Next(2)];
                            if (p == "Playful")     return new string[] { "~r~\"You already asked! I told you.\"", "~r~\"Still the same answer.\"" }[rng.Next(2)];
                            if (p == "Aggressive")  return new string[] { "~r~\"You already know. Don't waste my time.\"", "~r~\"I said what I said.\"" }[rng.Next(2)];
                            return new string[] { "~r~\"I already told you.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                        }
                        if (p == "Dominant")    return "\"Taking charge, mostly. I don't like being told what to do.\"";
                        if (p == "Shy")         return "\"I... I'm not sure I want to talk about that.\"";
                        if (p == "Playful")     return "\"Ooh. Lots of things. You'll find out.\"";
                        if (p == "Cold")        return "~r~\"That's not your business.\"";
                        if (p == "Sarcastic")   return "\"Things that probably wouldn't interest you.\"";
                        if (p == "Romantic")    return "\"Something tender. Something real.\"";
                        if (p == "依賴")       return "\"Whatever keeps someone close to me.\"";
                        if (p == "Aggressive")  return "\"Nothing boring. Nothing slow.\"";
                        return warm ? "\"Depends on the person. Ask me nicely.\"" : "~r~\"Why are you asking me that?\"";
                    case 3: // "Do you like doing... that with your mouth?"
                        // PrefBJ already known from activity or a previous answer — respond based on reality, not a fresh roll
                        if (d != null && d.PrefBJ != null)
                        {
                            bool bjKnown = d.PrefBJ == true;
                            bool bjAsked = (d.KnownTopics & (1L << 26)) != 0;
                            if (bjAsked) // already discussed verbally — treat as repeat
                            {
                                if (bjKnown) return new string[] { "~r~\"You already asked. Yes — I enjoy it.\"", "~r~\"Still yes. You don't forget things, do you?\"" }[rng.Next(2)];
                                if (p == "Cold")        return new string[] { "~r~\"I answered that. Stop asking.\"", "~r~\"Same answer.\"" }[rng.Next(2)];
                                if (p == "Shy")         return new string[] { "~r~\"Please don't make me say it again.\"", "~r~\"I told you already.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")   return new string[] { "~r~\"Still the same answer. Somehow you forgot.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already told you. No.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                            }
                            // Known from activity but never asked directly — she acknowledges it naturally
                            return bjKnown
                                ? "~r~\"I think you already know the answer to that.\""
                                : "~r~\"You should know by now it's not my thing.\"";
                        }
                        if (d != null && (d.KnownTopics & (1L << 26)) != 0) // asked before but pref still null (shouldn't happen, but safe fallback)
                        {
                            if (p == "Dominant")    return new string[] { "~r~\"You already have my answer on that.\"", "~r~\"I told you. Move on.\"" }[rng.Next(2)];
                            if (p == "Playful")     return new string[] { "~r~\"You already asked! The answer hasn't changed.\"", "~r~\"Still the same.\"" }[rng.Next(2)];
                            if (p == "Aggressive")  return new string[] { "~r~\"I said what I said. Move on.\"", "~r~\"You already know.\"" }[rng.Next(2)];
                            return new string[] { "~r~\"I already told you.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                        }
                        if (p == "Dominant")    return "\"On my terms. Not yours.\"";
                        if (p == "Shy")         return "~r~\"I'd... I'd rather not get into that.\"";
                        if (p == "Playful")     return "\"Maybe. You'd have to earn it.\"";
                        if (p == "Cold")        return "~r~\"That's personal. Drop it.\"";
                        if (p == "Sarcastic")   return "~r~\"Bold question. I'm going to pretend you didn't ask.\"";
                        if (p == "Aggressive")  return "\"Yeah. When I feel like it.\"";
                        if (p == "Romantic")    return "~r~\"Not with someone I barely know.\"";
                        if (p == "Manipulative") return "\"Maybe, if you play your cards right.\"";
                        if (riskiness >= 0.65)  return "\"Sure. I'm not shy about it.\"";
                        if (riskiness < 0.35)   return "~r~\"Not really. That's not my thing.\"";
                        return warm ? "\"If things go well.\"" : "~r~\"Bit forward, don't you think?\"";
                    case 4: // "Are you into it rough?"
                        // PrefRough already known from activity or a previous answer — respond based on reality, not a fresh roll
                        if (d != null && d.PrefRough != null)
                        {
                            bool roughKnown = d.PrefRough == true;
                            bool roughAsked = (d.KnownTopics & (1L << 28)) != 0;
                            if (roughAsked)
                            {
                                if (roughKnown) return new string[] { "~r~\"You already asked. Yes — I'm into it.\"", "~r~\"Still yes. Nothing's changed.\"" }[rng.Next(2)];
                                if (p == "Shy")         return new string[] { "~r~\"I... I already told you. Please.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                                if (p == "Romantic")    return new string[] { "~r~\"You already know how I feel about that.\"", "~r~\"Same answer. Still no.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")   return new string[] { "~r~\"You already asked. Still the same answer.\"", "~r~\"Retention isn't your strong suit.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already told you. No.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                            }
                            // Known from activity but never asked directly
                            return roughKnown
                                ? "~r~\"Pretty sure you already have your answer on that.\""
                                : "~r~\"That's not really my thing. You should've picked up on that.\"";
                        }
                        if (d != null && (d.KnownTopics & (1L << 28)) != 0)
                        {
                            if (p == "Aggressive")  return new string[] { "~r~\"I said what I said. Stop asking.\"", "~r~\"You already know.\"" }[rng.Next(2)];
                            if (p == "Dominant")    return new string[] { "~r~\"My preference hasn't changed. Ask something else.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                            if (p == "Cold")        return new string[] { "~r~\"Same answer as before.\"", "~r~\"I told you already.\"" }[rng.Next(2)];
                            return new string[] { "~r~\"I already told you.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                        }
                        if (p == "Dominant")    return "\"On my end, yes. Yours? We'll see.\"";
                        if (p == "Aggressive")  return "\"Yeah. Don't go soft on me.\"";
                        if (p == "Romantic")    return "~r~\"No. I like things slow. Intentional.\"";
                        if (p == "Shy")         return "~r~\"Absolutely not. That scares me.\"";
                        if (p == "Sweet")       return "~r~\"That's not really my thing, no.\"";
                        if (p == "依賴")       return "~r~\"No. I'd rather feel close, not... hurt.\"";
                        if (p == "Chaotic")     return "\"Sure. Keeps it interesting.\"";
                        if (p == "Playful")     return "\"Depends what you mean by rough.\"";
                        if (riskiness >= 0.75)  return "\"Can be. I'm not opposed.\"";
                        if (riskiness < 0.35)   return "~r~\"No. That's not something I'm into.\"";
                        return warm ? "\"Sometimes. Not always.\"" : "~r~\"I'd rather not go there.\"";
                    case 5: // "Would you ever take control?"
                        if (d != null && (d.KnownTopics & (1 << 29)) != 0)
                        {
                            if (p == "Dominant")    return new string[] { "~r~\"I already answered that. Obviously.\"", "~r~\"You already know I do.\"" }[rng.Next(2)];
                            if (p == "Shy")         return new string[] { "~r~\"You already asked me that...\"", "~r~\"I told you. No.\"" }[rng.Next(2)];
                            if (p == "Playful")     return new string[] { "~r~\"You already know my answer! Move on.\"", "~r~\"Still the same. Maybe.\"" }[rng.Next(2)];
                            if (p == "Sarcastic")   return new string[] { "~r~\"Same answer. You really don't pay attention, do you?\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                            if (p == "Cold")        return new string[] { "~r~\"You already know.\"", "~r~\"I answered that.\"" }[rng.Next(2)];
                            if (p == "Aggressive")  return new string[] { "~r~\"You already asked. You have your answer.\"", "~r~\"Same thing I said before.\"" }[rng.Next(2)];
                            return new string[] { "~r~\"I already told you.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                        }
                        if (p == "Dominant")    return "\"Always.\"";
                        if (p == "Aggressive")  return "\"If you can't keep up, yeah.\"";
                        if (p == "Shy")         return "~r~\"Oh no. No! You lead.\"";
                        if (p == "Playful")     return "\"Maybe. Might be fun to surprise you.\"";
                        if (p == "Cold")        return "\"If I wanted to.\"";
                        if (p == "Manipulative") return "\"I always do, whether you notice it or not.\"";
                        if (p == "Romantic")    return "~r~\"I'm more of a... let things unfold naturally type.\"";
                        if (confidence >= 0.75) return "\"Sure. I don't mind leading.\"";
                        if (confidence < 0.30)  return "~r~\"I'd rather not. Too much pressure.\"";
                        return warm ? "\"Maybe. If I'm in the right mood.\"" : "~r~\"Probably not.\"";
                    case 6: // "What do you absolutely not want?"
                        if (d != null && (d.KnownTopics & (1L << 38)) != 0)
                        {
                            if (p == "Cold")        return new string[] { "~r~\"I said it once. That's it.\"", "~r~\"You already know my limits.\"" }[rng.Next(2)];
                            if (p == "Dominant")    return new string[] { "~r~\"You already know my limits. Don't push them.\"", "~r~\"I told you. You heard me.\"" }[rng.Next(2)];
                            if (p == "Shy")         return new string[] { "~r~\"You already know. Please don't make me repeat it.\"", "~r~\"I told you already.\"" }[rng.Next(2)];
                            if (p == "Sarcastic")   return new string[] { "~r~\"Same things as before. Nothing's changed.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                            if (p == "依賴")       return new string[] { "~r~\"You already asked. I told you.\"", "~r~\"Please don't make me say it again.\"" }[rng.Next(2)];
                            if (p == "Romantic")    return new string[] { "~r~\"You already asked me that.\"", "~r~\"I told you. That hasn't changed.\"" }[rng.Next(2)];
                            if (p == "Aggressive")  return new string[] { "~r~\"I told you what I won't do. Don't ask again.\"", "~r~\"You already know.\"" }[rng.Next(2)];
                            return new string[] { "~r~\"I already told you.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                        }
                        if (p == "Romantic")    return "\"Anything that doesn't feel real. Or rushed.\"";
                        if (p == "Shy")         return "\"Anything too... exposed. Or loud.\"";
                        if (p == "Aggressive")  return "\"Being ignored. Boredom.\"";
                        if (p == "Cold")        return "\"Clingy behaviour after. Keep it clean.\"";
                        if (p == "Dominant")    return "\"Being told what to do. That's final.\"";
                        if (p == "依賴")       return "\"Being left feeling empty. I hate that.\"";
                        if (p == "Chaotic")     return "\"Being predictable.\"";
                        if (p == "Sweet")       return "\"Anything that feels mean or cold. Just... be kind.\"";
                        if (p == "Gold Digger") return "\"Wasting my time on someone who can't deliver.\"";
                        return warm ? "\"Being disrespected. That shuts everything down.\"" : "\"Things I didn't agree to.\"";
                    case 7: // "Do you want this casual?"
                        if (d != null && (d.KnownTopics & (1 << 30)) != 0)
                        {
                            if (p == "Independent")  return new string[] { "~r~\"Yes. I told you. Keep it clean.\"", "~r~\"I already answered. Yes. Casual.\"" }[rng.Next(2)];
                            if (p == "Romantic")     return new string[] { "~r~\"You already asked. Still not what I want.\"", "~r~\"I gave you an honest answer. Don't make me repeat it.\"" }[rng.Next(2)];
                            if (p == "Cold")         return new string[] { "~r~\"I said what I said.\"", "~r~\"Same answer.\"" }[rng.Next(2)];
                            if (p == "依賴")        return new string[] { "~r~\"You already know I don't want casual. Please stop asking.\"", "~r~\"I already told you.\"" }[rng.Next(2)];
                            if (p == "Sarcastic")    return new string[] { "~r~\"You already know how I answered that.\"", "~r~\"Same as last time.\"" }[rng.Next(2)];
                            return new string[] { "~r~\"I already told you.\"", "~r~\"We went over this.\"" }[rng.Next(2)];
                        }
                        if (attachment >= 0.70) return "~r~\"Not really. But I'll take what I can get.\"";
                        if (p == "Independent") return "\"Yes. Keep it clean.\"";
                        if (p == "Romantic")    return "~r~\"Not if I'm being honest.\"";
                        return warm ? "\"Let's just see where it goes.\"" : "~r~\"I don't label things.\"";
                }
            }
            else if (branch == 1) // Test Waters
            {
                switch (item)
                {
                    case 0: // "You look hard to resist."
                        if (p == "Cold")        return "~r~\"Then try harder.\"";
                        if (p == "Sarcastic")   return "~r~\"And yet here you are. Resisting just fine.\"";
                        if (p == "Shy")         return "\"Oh... thank you. That's... nice to hear.\"";
                        if (p == "Playful")     return "\"So don't resist. Problem solved.\"";
                        if (p == "Dominant")    return "\"I know. That's kind of the point.\"";
                        if (p == "Romantic")    return "\"You make me feel something when you say that.\"";
                        if (p == "Manipulative") return "\"Good. Stay that way.\"";
                        if (rel >= 30 || warm)  return "\"...I was just thinking the same about you.\"";
                        return "~r~\"That's a line. A decent one. But still a line.\"";
                    case 1: // "I want to be closer to you."
                        if (p == "Cold")        return "~r~\"Noted. Don't act on it.\"";
                        if (p == "Shy")         return "\"I... okay. I don't mind that.\"";
                        if (p == "Romantic")    return "\"I want that too. I just don't say it out loud.\"";
                        if (p == "依賴")       return "\"Then stay. Please don't go.\"";
                        if (p == "Independent") return "~r~\"Just don't get too close. I need my space.\"";
                        if (p == "Dominant")    return "\"Then come closer. But on my terms.\"";
                        if (p == "Sarcastic")   return "~r~\"How poetic. Was that rehearsed?\"";
                        if (rel >= 40 || warm)  return "\"...Come here then.\"";
                        return "~r~\"You barely know me.\"";
                    case 2: // "Do you want this too?"
                        if (p == "Cold")        return "~r~\"Define 'this'.\"";
                        if (p == "Shy")         return "\"I... yes. I think so.\"";
                        if (p == "Romantic")    return "\"I do. More than I should probably admit.\"";
                        if (p == "Dominant")    return "\"Maybe. But I decide when.\"";
                        if (p == "Playful")     return "\"That depends. Are you going to make it worth saying yes?\"";
                        if (p == "Manipulative") return "\"I might. What do you have to offer?\"";
                        if (p == "Sarcastic")   return "~r~\"I haven't decided yet. Stop asking.\"";
                        if (p == "Aggressive")  return "\"Stop asking. You'll know.\"";
                        if (rel >= 50)          return "\"...Yeah. I do.\"";
                        if (rel >= 25 && warm)  return "\"I'm not saying no.\"";
                        return "~r~\"You haven't done enough to make me say yes.\"";
                }
            }

            return "\"...\"";
        }

        /// <summary>
        /// Return an NPC response line for the selected conversation branch and item.
        /// branch 0=GetToKnow, 1=SmallTalk, 2=CheckMood, 3=Flirt, 4=MakeHerMine, 5=Personal, 6=Business.
        /// Response starts with ~r~ for negative/deflecting, otherwise positive.
        /// </summary>
        /// <summary>All dialogue responses for Prostitution A-Life conversation branches 0-3.
        /// Hooker-specific tone: street-smart, professional, guarded but can warm up.
        /// Positive responses have no leading colour tag; negative start with ~r~.</summary>
        private string GetProstConvResponse(int branch, int item, ALifePedData d)
        {
            string p = (d != null && d.Personality != null) ? d.Personality : "";
            int rep = (d != null) ? d.Reputation : 0;
            PersonalityProfile prof = (d != null) ? GetProfile(d.Personality) : null;
            bool warm = IsWarmPersonality(d);
            bool opensUp = rep >= 30; // Regular tier: she's more candid
            string mood  = (d != null && d.Mood != null && d.Mood.Length > 0) ? d.Mood : "Relaxed";
            bool isNight = IsNight();

            // Mood shifts
            if (d != null)
            {
                if (branch == 3) d.Mood = "Playful";
                else if (branch == 2) { /* mood reveal — don't change it */ }
                else if (warm) d.Mood = "Relaxed";
            }

            // ── Hostile mode: rep -1 — all questions get a cold/dismissive negative response ──
            if (rep <= -1)
            {
                switch (branch)
                {
                    case 0: // Get to Know Her
                        switch (item)
                        {
                            case 0: // "What do I call you?"
                                if (d != null && d.NameKnown)
                                {
                                    if (p == "Aggressive") return "~r~\"You know it. Don't push me.\"";
                                    if (p == "Cold")       return "~r~\"You already know. Leave it.\"";
                                    return "~r~\"You know my name. What do you want?\"";
                                }
                                if (p == "Aggressive")   return "~r~\"Not your business. Back off.\"";
                                if (p == "Dominant")     return "~r~\"You don't get that from me.\"";
                                if (p == "Cold")         return "~r~\"You don't need it.\"";
                                if (p == "Sarcastic")    return "~r~\"That's not happening today.\"";
                                if (p == "Street Smart") return "~r~\"You're not getting that. Move on.\"";
                                return "~r~\"I don't give that out.\"";
                            case 1: // "Where you from?"
                                if (p == "Aggressive")   return "~r~\"None of your business.\"";
                                if (p == "Cold")         return "~r~\"Does it matter?\"";
                                if (p == "Sarcastic")    return "~r~\"Why, you writing a book?\"";
                                if (p == "Dominant")     return "~r~\"Not something I'm sharing with you.\"";
                                return "~r~\"That's not something I'm getting into right now.\"";
                            case 2: // "You always work this area?"
                                if (p == "Aggressive")   return "~r~\"Why are you keeping track?\"";
                                if (p == "Cold")         return "~r~\"Around. That's all you're getting.\"";
                                if (p == "Sarcastic")    return "~r~\"You clocking my schedule now?\"";
                                return "~r~\"Don't worry about where I work.\"";
                            case 3: // "How long you been doing this?"
                                if (p == "Aggressive")   return "~r~\"That is none of your business.\"";
                                if (p == "Cold")         return "~r~\"Why would I tell you that?\"";
                                if (p == "Shy")          return "~r~\"I really don't want to talk about that.\"";
                                if (p == "Sarcastic")    return "~r~\"Long enough to know when someone's wasting my time.\"";
                                return "~r~\"That's not your business.\"";
                        }
                        break;
                    case 1: // Small Talk
                        switch (item)
                        {
                            case 0: // "Slow day/night?"
                                if (p == "Aggressive")   return "~r~\"I'm not in the mood for small talk.\"";
                                if (p == "Cold")         return "~r~\"You really asking me that right now?\"";
                                if (p == "Sarcastic")    return "~r~\"Getting worse by the second.\"";
                                return "~r~\"Not really interested in chatting.\"";
                            case 1: // "You been out long?"
                                if (p == "Aggressive")   return "~r~\"Why do you care?\"";
                                if (p == "Cold")         return "~r~\"That's not your concern.\"";
                                if (p == "Sarcastic")    return "~r~\"Long enough to be done with this conversation.\"";
                                return "~r~\"I'd rather not talk right now.\"";
                            case 2: // "Anyone giving you trouble?"
                                if (p == "Aggressive")   return "~r~\"Yeah. You, right now.\"";
                                if (p == "Dominant")     return "~r~\"None of your concern. Back off.\"";
                                if (p == "Sarcastic")    return "~r~\"You're testing that theory right now.\"";
                                return "~r~\"Not looking for conversation.\"";
                            case 3: // "You working alone?"
                                if (p == "Aggressive")   return "~r~\"Stop asking me questions.\"";
                                if (p == "Cold")         return "~r~\"That's not something you need to know.\"";
                                if (p == "Sarcastic")    return "~r~\"Why? You keeping an eye out for me? Don't.\"";
                                return "~r~\"I'm not getting into that with you.\"";
                        }
                        break;
                    case 2: // Check Mood
                        switch (item)
                        {
                            case 0: // "How you holding up?"
                                if (p == "Aggressive")   return "~r~\"Not great, and you're not helping.\"";
                                if (p == "Cold")         return "~r~\"Fine. Don't ask again.\"";
                                if (p == "Sarcastic")    return "~r~\"Better before you walked over.\"";
                                return "~r~\"Not in the mood for this.\"";
                            case 1: // "You okay?"
                                if (p == "Aggressive")   return "~r~\"I said I'm fine. Leave me alone.\"";
                                if (p == "Cold")         return "~r~\"Don't ask me that.\"";
                                if (p == "Sarcastic")    return "~r~\"Was until a minute ago.\"";
                                return "~r~\"I'd rather you didn't.\"";
                            case 2: // "You seem stressed."
                                if (p == "Aggressive")   return "~r~\"Yeah, I wonder why.\"";
                                if (p == "Cold")         return "~r~\"I'm not talking about that.\"";
                                if (p == "Sarcastic")    return "~r~\"Gee, what gave it away.\"";
                                return "~r~\"I'll handle it. Mind your own business.\"";
                            case 3: // "You look good tonight/today."
                                if (p == "Aggressive")   return "~r~\"Save it.\"";
                                if (p == "Cold")         return "~r~\"That's not going to work.\"";
                                if (p == "Sarcastic")    return "~r~\"Really? That's your move right now?\"";
                                if (p == "Dominant")     return "~r~\"Don't try to flatter your way in.\"";
                                return "~r~\"Not the time for that.\"";
                        }
                        break;
                    case 3: // Flirt
                        switch (item)
                        {
                            case 0: // "You're hard to walk past."
                                if (p == "Aggressive")   return "~r~\"Then keep walking.\"";
                                if (p == "Cold")         return "~r~\"Try harder.\"";
                                if (p == "Sarcastic")    return "~r~\"Most people manage it. Try again.\"";
                                if (p == "Dominant")     return "~r~\"Not interested. Move on.\"";
                                return "~r~\"I'm not in the mood for that.\"";
                            case 1: // "I always look for you out here."
                                if (p == "Aggressive")   return "~r~\"Stop doing that.\"";
                                if (p == "Cold")         return "~r~\"I don't need you looking for me.\"";
                                if (p == "Sarcastic")    return "~r~\"Maybe take the hint and stop.\"";
                                if (p == "嫉妒")      return "~r~\"And what am I supposed to do with that?\"";
                                return "~r~\"I'd rather you didn't.\"";
                            case 2: // "You've got a way about you."
                                if (p == "Aggressive")   return "~r~\"Save the compliments.\"";
                                if (p == "Cold")         return "~r~\"That's not going to get you anywhere.\"";
                                if (p == "Sarcastic")    return "~r~\"Yeah, it's called not wanting to talk to you.\"";
                                if (p == "Dominant")     return "~r~\"Flattery isn't going to change anything.\"";
                                return "~r~\"Not right now.\"";
                            case 3: // "You make this worth coming back for."
                                if (p == "Aggressive")   return "~r~\"Then don't come back.\"";
                                if (p == "Cold")         return "~r~\"That's not my problem.\"";
                                if (p == "Sarcastic")    return "~r~\"High praise. Still a no.\"";
                                if (p == "Dominant")     return "~r~\"I don't need your approval to feel worth it.\"";
                                return "~r~\"Not interested.\"";
                        }
                        break;
                }
                return "~r~\"...\"";
            }

            switch (branch)
            {
                // ── 0: Get to Know Her ──────────────────────────────────────────────────────
                case 0:
                    switch (item)
                    {
                        case 0: // "What do I call you?" — name reveal
                            if (d != null && d.NameKnown)
                                return new string[] { "~r~\"You already know that.\"", "~r~\"We've been through this.\"" }[rng.Next(2)];
                            // Asked before but she refused — repeat ask penalty path
                            if (d != null && (d.KnownTopics & (1L << 39)) != 0)
                            {
                                if (p == "Dominant")     return new string[] { "~r~\"I said no. Stop asking.\"", "~r~\"You don't take hints well.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"I already said I don't give that out.\"", "~r~\"You heard me the first time.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"Same answer. Still no.\"", "~r~\"I already told you.\"" }[rng.Next(2)];
                                if (p == "Shy")          return new string[] { "~r~\"I... already said no.\"", "~r~\"Please stop asking me that.\"" }[rng.Next(2)];
                                if (p == "Aggressive")   return new string[] { "~r~\"Stop asking.\"", "~r~\"I told you. Let it go.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already told you I don't give that out.\"", "~r~\"Still not telling you.\"" }[rng.Next(2)];
                            }
                            if (p == "Dominant")   return "\"" + (d != null ? d.Name : "?") + ". Don't forget it.\"";
                            if (p == "Shy")        return warm ? "\"...It's " + (d != null ? d.Name : "?") + ".\"" : "~r~\"I don't usually give that out.\"";
                            if (p == "Cold")       return opensUp ? "\"" + (d != null ? d.Name : "?") + ".\"" : "~r~\"You don't need it.\"";
                            if (p == "Sarcastic")  return "\"Most regulars don't ask. But... " + (d != null ? d.Name : "?") + ".\"";
                            return (warm || opensUp) ? "\"" + (d != null ? d.Name : "?") + ". Nice of you to ask.\"" : "~r~\"I don't give that out.\"";
                        case 1: // "Where you from?"
                            if (d != null && (d.KnownTopics & (1 << 0)) != 0)
                            {
                                if (p == "Sarcastic")    return new string[] { "~r~\"Still from the same place.\"", "~r~\"Already told you that.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"I already answered that.\"", "~r~\"You heard me.\"" }[rng.Next(2)];
                                if (p == "Mysterious")   return new string[] { "~r~\"Still the same place.\"", "~r~\"My answer hasn't changed.\"" }[rng.Next(2)];
                                if (p == "Street Smart") return new string[] { "~r~\"Said what I said.\"", "~r~\"You already heard this.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked me that.\"", "~r~\"We went over this.\"" }[rng.Next(2)];
                            }
                            if (p == "Street Smart") return opensUp ? "\"South side, born and raised. I know every block.\"" : "\"Around. Why?\"";
                            if (p == "Mysterious")   return "\"Somewhere that doesn't really exist anymore.\"";
                            if (p == "Cold")         return opensUp ? "\"Somewhere else.\"" : "~r~\"Does it matter?\"";
                            if (p == "Chaotic")      return "\"Everywhere. Nowhere. I move around a lot.\"";
                            if (p == "Sarcastic")    return "\"Is this where we pretend this is a date?\"";
                            return (warm || opensUp) ? "\"Not from here, but I've been here long enough.\"" : "~r~\"That's a bit personal.\"";
                        case 2: // "You always work this area?"
                            if (d != null && (d.KnownTopics & (1 << 1)) != 0)
                            {
                                if (p == "Chaotic")      return new string[] { "~r~\"Still wherever I feel like.\"", "~r~\"You already know my answer.\"" }[rng.Next(2)];
                                if (p == "Gold Digger")  return new string[] { "~r~\"Same as before.\"", "~r~\"I already told you.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Still the same stretch.\"", "~r~\"You really need to ask again?\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked me that.\"", "~r~\"We covered that.\"" }[rng.Next(2)];
                            }
                            if (p == "Street Smart") return "\"This and a couple other spots. Depends on the night.\"";
                            if (p == "Chaotic")      return "\"Wherever I feel like. I don't keep a schedule.\"";
                            if (p == "Cold")         return "\"Around. Yeah.\"";
                            if (p == "Mysterious")   return "\"I go where the night takes me.\"";
                            if (p == "Gold Digger")  return "\"The better areas mostly. Quality matters.\"";
                            return (warm || opensUp) ? "\"Pretty much. I know this stretch well.\"" : "\"Sometimes. Why you keeping track?\"";
                        case 3: // "How long you been doing this?"
                            if (d != null && (d.KnownTopics & (1 << 2)) != 0)
                            {
                                if (p == "Sarcastic")    return new string[] { "~r~\"Still as long as the last time you asked.\"", "~r~\"You already heard this.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"I told you. Move on.\"", "~r~\"I already answered that.\"" }[rng.Next(2)];
                                if (p == "Shy")          return new string[] { "~r~\"I... already said I don't want to talk about it.\"", "~r~\"Please stop asking.\"" }[rng.Next(2)];
                                if (p == "Gold Digger")  return new string[] { "~r~\"Still the same answer.\"", "~r~\"We went over this.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked me that.\"", "~r~\"Same answer as before.\"" }[rng.Next(2)];
                            }
                            if (p == "Gold Digger")  return opensUp ? "\"Long enough to know exactly what I'm worth.\"" : "\"Long enough. Why?\"";
                            if (p == "Street Smart") return "\"Long enough. I know the game.\"";
                            if (p == "Shy")          return "~r~\"I'd rather not talk about that.\"";
                            if (p == "Cold")         return "~r~\"Why would I tell you that?\"";
                            if (p == "Sarcastic")    return "\"Long enough that you're not the first one to ask.\"";
                            if (p == "Manipulative") return opensUp ? "\"A while. I've learned a lot about people.\"" : "\"Long enough.\"";
                            return (warm || opensUp) ? "\"A few years. Don't make it a thing.\"" : "~r~\"That's not really your business.\"";
                    }
                    break;

                // ── 1: Small Talk ────────────────────────────────────────────────────────────
                case 1:
                    switch (item)
                    {
                        case 0: // "Slow night?" / "Slow day?"
                            if (d != null && (d.KnownTopics & (1 << 6)) != 0)
                            {
                                if (p == "Sarcastic")    return new string[] { "~r~\"Still the same answer it was last time.\"", "~r~\"Asked. Answered. Moving on?\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"You know the answer.\"", "~r~\"Same as before.\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"You already asked me that one.\"", "~r~\"Ha, running out of material?\"" }[rng.Next(2)];
                                if (p == "Gold Digger")  return new string[] { "~r~\"Same answer. Bring something new.\"", "~r~\"I already answered that.\"" }[rng.Next(2)];
                                return isNight
                                    ? new string[] { "~r~\"You already asked about the night.\"", "~r~\"We covered that.\"" }[rng.Next(2)]
                                    : new string[] { "~r~\"You already asked that.\"", "~r~\"We covered that.\"" }[rng.Next(2)];
                            }
                            if (p == "Gold Digger")  return "\"Until you walked over.\"";
                            if (p == "Party Girl")   return "\"Slow? Please. There's always something.\"";
                            if (p == "Cold")         return isNight ? "\"It's a night. Who's asking?\"" : "\"It's a day. Who's asking?\"";
                            if (p == "Sarcastic")    return "\"Oh it was dead until you showed up and asked me that.\"";
                            if (p == "Playful")      return isNight ? "\"Was quiet. Then you showed up.\"" : "\"Picking up.\"";
                            return (warm || opensUp) ? "\"Getting better.\"" : (isNight ? "\"It's a night. Yeah.\"" : "\"It's a day. Yeah.\"");
                        case 1: // "You staying out late?"
                            if (d != null && (d.KnownTopics & (1 << 7)) != 0)
                            {
                                if (p == "Sarcastic")    return new string[] { "~r~\"Still planning to stay as long as I said.\"", "~r~\"You already asked that.\"" }[rng.Next(2)];
                                if (p == "Playful")      return new string[] { "~r~\"Ha, checking up on me already?\"", "~r~\"I told you. I'm not going anywhere yet.\"" }[rng.Next(2)];
                                if (p == "Mysterious")   return new string[] { "~r~\"My plans haven't changed since you asked.\"", "~r~\"Still here. That answer enough?\"" }[rng.Next(2)];
                                if (p == "Street Smart") return new string[] { "~r~\"Said what I said. Same answer.\"", "~r~\"You already know.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"I already answered that.\"", "~r~\"You already asked me this.\"" }[rng.Next(2)];
                            }
                            if (p == "Gold Digger")  return "\"As long as business keeps coming.\"";
                            if (p == "Playful")      return "\"Why, you got plans for me?\"";
                            if (p == "Cold")         return "\"Long as I need to.\"";
                            if (p == "Sarcastic")    return "\"That your way of asking me to stick around?\"";
                            return (warm || opensUp) ? "\"For a while yet.\"" : "\"Depends.\"";
                        case 2: // "Anyone giving you trouble?"
                            if (d != null && (d.KnownTopics & (1 << 8)) != 0)
                            {
                                if (p == "Aggressive")   return new string[] { "~r~\"Still handled. Still don't need help.\"", "~r~\"I told you — I've got it.\"" }[rng.Next(2)];
                                if (p == "Dominant")     return new string[] { "~r~\"Still handled. Stop asking.\"", "~r~\"I gave you my answer.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Nope. Still the same answer.\"", "~r~\"You really need to hear it twice?\"" }[rng.Next(2)];
                                if (p == "Shy")          return new string[] { "~r~\"I... already said no. Why do you keep asking?\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                                if (p == "Manipulative") return new string[] { "~r~\"I said I handled it. Why are you so interested?\"", "~r~\"Already answered that.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"We covered that already.\"", "~r~\"I already told you.\"" }[rng.Next(2)];
                            }
                            if (p == "Aggressive")   return "\"Let them try.\"";
                            if (p == "Dominant")     return "\"I handle it.\"";
                            if (p == "Street Smart") return opensUp ? "\"Had one earlier. Sorted it.\"" : "\"Nothing I can't deal with.\"";
                            if (p == "Chaotic")      return "\"Always someone. Part of the job.\"";
                            if (p == "Cold")         return "\"Not yet.\"";
                            if (p == "Sarcastic")    return "\"Define trouble.\"";
                            if (p == "Shy")          return "\"Not really... why?\"";
                            if (p == "Manipulative") return "\"Nothing I haven't handled. Why, you offering?\"";
                            return (warm || opensUp) ? "\"No, it's been fine.\"" : "\"I can look after myself.\"";
                        case 3: // "You working alone?"
                            if (d != null && (d.KnownTopics & (1 << 9)) != 0)
                            {
                                if (p == "Dominant")     return new string[] { "~r~\"I told you. Still alone. Still fine with that.\"", "~r~\"You already know my answer.\"" }[rng.Next(2)];
                                if (p == "Independent")  return new string[] { "~r~\"Same answer as before.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                                if (p == "Mysterious")   return new string[] { "~r~\"I already gave you as much as I'm giving.\"", "~r~\"That question still has the same answer.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"Yes, still alone. Like I said.\"", "~r~\"You checked already. Still yes.\"" }[rng.Next(2)];
                                if (p == "Shy")          return new string[] { "~r~\"I... yes. I already said that.\"", "~r~\"You already asked.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already know.\"", "~r~\"I told you that already.\"" }[rng.Next(2)];
                            }
                            if (p == "Mysterious")   return "\"Depends what you mean by alone.\"";
                            if (p == "Independent")  return "\"Yeah. I don't need anyone watching my back.\"";
                            if (p == "Shy")          return "\"Yeah... most nights.\"";
                            if (p == "Street Smart") return opensUp ? (isNight ? "\"Tonight yeah. I know how to handle myself.\"" : "\"Right now, yeah. I know how to handle myself.\"") : (isNight ? "\"Tonight yeah.\"" : "\"Right now, yeah.\"");
                            return (warm || opensUp) ? (isNight ? "\"Tonight I am, yeah.\"" : "\"Right now I am, yeah.\"") : "~r~\"Why does that matter to you?\"";
                    }
                    break;

                // ── 2: Check Mood ────────────────────────────────────────────────────────────
                case 2:
                    if (d != null) d.Mood = mood;
                    switch (item)
                    {
                        case 0: // "How you holding up?"
                            if (d != null && (d.KnownTopics & (1L << 12)) != 0)
                            {
                                if (mood == "Annoyed")  return new string[] { "~r~\"I already told you. Not great.\"", "~r~\"Same answer. Still not better.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")   return new string[] { "~r~\"You really asking again?\"", "~r~\"Still the same as when you asked.\"" }[rng.Next(2)];
                                if (p == "Cold")        return new string[] { "~r~\"I already answered that.\"", "~r~\"We covered that.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked me that.\"", "~r~\"Same answer as before.\"" }[rng.Next(2)];
                            }
                            if (mood == "Annoyed")  return "~r~\"Been better. Let's just get to it.\"";
                            if (mood == "Alert")  return isNight ? "\"Careful out here tonight. That's how I'm holding up.\"" : "\"Careful out here. That's how I'm holding up.\"";
                            if (mood == "Playful")  return "\"Good actually. You showed up at the right time.\"";
                            if (mood == "Needy")    return "\"...I'm okay. Are you okay?\"";
                            if (mood == "Jealous")  return "\"Fine. Why? Did something happen?\"";
                            return (warm || opensUp) ? "\"Can't complain.\"" : "\"I'm here, aren't I?\"";
                        case 1: // "You okay?"
                            if (d != null && (d.KnownTopics & (1L << 13)) != 0)
                            {
                                if (mood == "Annoyed")  return new string[] { "~r~\"I said I'm fine. Still fine. Stop asking.\"", "~r~\"You already asked me that.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")   return new string[] { "~r~\"You asked. I answered. Moving on?\"", "~r~\"Still okay. Same as before.\"" }[rng.Next(2)];
                                if (p == "Cold")        return new string[] { "~r~\"Already told you. Yes.\"", "~r~\"We covered that.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already asked me that.\"", "~r~\"Still the same answer.\"" }[rng.Next(2)];
                            }
                            if (mood == "Annoyed")  return "~r~\"I said I'm fine. Don't push it.\"";
                            if (mood == "Alert")  return "\"Why? I look like something's wrong?\"";
                            if (mood == "Needy")    return "\"Better now.\"";
                            if (mood == "Playful")  return "\"Better than okay. You here for business or just conversation?\"";
                            return (warm || opensUp) ? "\"Yeah. You?\"" : "\"Always.\"";
                        case 2: // "You seem stressed."
                            if (d != null && (d.KnownTopics & (1L << 14)) != 0)
                            {
                                if (mood == "Annoyed")  return new string[] { "~r~\"I told you — I'm handling it.\"", "~r~\"You already said that.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")   return new string[] { "~r~\"Still 'stressed' apparently. You covered that.\"", "~r~\"We went over this.\"" }[rng.Next(2)];
                                if (p == "Cold")        return new string[] { "~r~\"I already addressed that.\"", "~r~\"I told you. It passes.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already said that.\"", "~r~\"You mentioned that already.\"" }[rng.Next(2)];
                            }
                            if (mood == "Annoyed")  return "\"It's been a night. I'll handle it.\"";
                            if (mood == "Alert")  return "\"I'm not stressed. I'm alert. Different thing.\"";
                            if (mood == "Relaxed")  return "\"I'm good. Not everything means something.\"";
                            if (mood == "Jealous")  return "\"Something's on my mind. Forget it.\"";
                            return "\"Maybe. It passes.\"";
                        case 3: // "You look good tonight." / "You look good today."
                            if (d != null && (d.KnownTopics & (1L << 15)) != 0)
                            {
                                if (p == "Sarcastic")   return new string[] { "~r~\"You said that already.\"", "~r~\"Heard it. Thanks. Moving on?\"" }[rng.Next(2)];
                                if (p == "Dominant")    return new string[] { "~r~\"You mentioned that. I know.\"", "~r~\"I heard you the first time.\"" }[rng.Next(2)];
                                if (mood == "Annoyed")  return new string[] { "~r~\"Still not the night for it.\"", "~r~\"I said not now.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already said that.\"", "~r~\"I heard you the first time.\"" }[rng.Next(2)];
                            }
                            if (mood == "Playful")  return "\"I know. But thanks for saying it.\"";
                            if (mood == "Annoyed")  return "~r~\"Not the night for compliments.\"";
                            if (mood == "Alert")  return "\"That supposed to get you somewhere?\"";
                            if (p == "Dominant")    return "\"I know. Is that all?\"";
                            if (p == "Sarcastic")   return "\"High praise from a man on a corner.\"";
                            if (p == "Shy")         return "\"...Thank you.\"";
                            return (warm || opensUp) ? "\"Thank you. That's nice.\"" : "\"Yeah. I know.\"";
                    }
                    break;

                // ── 3: Flirt ─────────────────────────────────────────────────────────────────
                case 3:
                    switch (item)
                    {
                        case 0: // "You're hard to walk past."
                            if (d != null && (d.KnownTopics & (1L << 17)) != 0)
                            {
                                if (p == "Sarcastic")   return new string[] { "~r~\"You said that last time too.\"", "~r~\"Running out of lines?\"" }[rng.Next(2)];
                                if (p == "Playful")     return new string[] { "~r~\"Ha, you used that one already.\"", "~r~\"Still true, but you said it.\"" }[rng.Next(2)];
                                if (p == "Cold")        return new string[] { "~r~\"You already said that.\"", "~r~\"I heard you.\"" }[rng.Next(2)];
                                if (p == "Dominant")    return new string[] { "~r~\"You mentioned that. Moving on?\"", "~r~\"I know. You said that.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already said that.\"", "~r~\"You used that one already.\"" }[rng.Next(2)];
                            }
                            if (p == "Dominant")    return "\"Then don't. You made the right call.\"";
                            if (p == "Shy")         return "\"Oh... that's sweet.\"";
                            if (p == "Gold Digger") return "\"Then it's your lucky night.\"";
                            if (p == "Sarcastic")   return "\"I've heard worse pickup lines.\"";
                            if (p == "Cold")        return "~r~\"Good. So you stopped.\"";
                            if (p == "Playful")     return "\"I hear that a lot. Still feels good though.\"";
                            return (warm || opensUp) ? "\"Ha. Thank you.\"" : "~r~\"Okay.\"";
                        case 1: // "I always look for you out here."
                            if (d != null && (d.KnownTopics & (1L << 18)) != 0)
                            {
                                if (p == "依賴")        return new string[] { "~r~\"You already told me that. I remember.\"", "~r~\"You said that before.\"" }[rng.Next(2)];
                                if (p == "嫉妒")      return new string[] { "~r~\"You mentioned that. And the other girls?\"", "~r~\"You already said that.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")    return new string[] { "~r~\"You said that already. I remember.\"", "~r~\"I heard you the first time.\"" }[rng.Next(2)];
                                if (p == "Cold")         return new string[] { "~r~\"You said that.\"", "~r~\"I know. You mentioned it.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already told me that.\"", "~r~\"I heard you the first time.\"" }[rng.Next(2)];
                            }
                            if (p == "依賴")        return "\"Do you? That actually means something to me.\"";
                            if (p == "Manipulative") return "\"Good. I was hoping you'd come back.\"";
                            if (p == "Independent")  return "\"You know where to find me.\"";
                            if (p == "嫉妒")      return "\"Yeah? And the other girls out here?\"";
                            if (p == "Cold")         return "\"I figured.\"";
                            if (p == "Shy")          return "\"Really? I... thank you.\"";
                            return (warm || opensUp) ? "\"I know. I look out for you too.\"" : "\"You know where I am.\"";
                        case 2: // "You've got a way about you."
                            if (d != null && (d.KnownTopics & (1L << 19)) != 0)
                            {
                                if (p == "Mysterious")  return new string[] { "~r~\"You already noticed. I appreciated it.\"", "~r~\"You mentioned that.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")   return new string[] { "~r~\"'A way.' Still the same way.\"", "~r~\"You said that before.\"" }[rng.Next(2)];
                                if (p == "Playful")     return new string[] { "~r~\"Ha, you already used that one.\"", "~r~\"I know! You said that.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already said that.\"", "~r~\"I heard you the first time.\"" }[rng.Next(2)];
                            }
                            if (p == "Mysterious")  return "\"Most people notice but can't explain it. I like that you tried.\"";
                            if (p == "Sarcastic")   return "\"'A way.' That's one word for it.\"";
                            if (p == "Dominant")    return "\"I know. Very intentional.\"";
                            if (p == "Cold")        return "\"You're not wrong.\"";
                            if (p == "Playful")     return "\"Ha! 'A way.' I love that.\"";
                            if (p == "Shy")         return "\"I'm not sure what that means, but... thank you.\"";
                            return (warm || opensUp) ? "\"That's kind of you.\"" : "~r~\"Sure.\"";
                        case 3: // "You make this worth coming back for."
                            if (d != null && (d.KnownTopics & (1L << 20)) != 0)
                            {
                                if (p == "Gold Digger") return new string[] { "~r~\"Yes, that's the plan. You mentioned it.\"", "~r~\"I know. You said that.\"" }[rng.Next(2)];
                                if (p == "Sarcastic")   return new string[] { "~r~\"High praise. Still the same praise.\"", "~r~\"You said that already.\"" }[rng.Next(2)];
                                if (p == "依賴")       return new string[] { "~r~\"You already said that. Keep saying it though.\"", "~r~\"I heard you. Still true?\"" }[rng.Next(2)];
                                if (p == "Cold")        return new string[] { "~r~\"You said that.\"", "~r~\"I heard you.\"" }[rng.Next(2)];
                                return new string[] { "~r~\"You already said that.\"", "~r~\"I heard you the first time.\"" }[rng.Next(2)];
                            }
                            if (p == "Gold Digger") return "\"That's the whole plan.\"";
                            if (p == "Romantic")    return "\"That's... actually really nice to hear.\"";
                            if (p == "依賴")       return "\"Keep saying things like that.\"";
                            if (p == "Cold")        return "\"I know my value.\"";
                            if (p == "Sarcastic")   return "\"High praise. You sure?\"";
                            if (p == "Sweet")       return "\"Aw. That genuinely makes me happy.\"";
                            return (warm || opensUp) ? "\"I appreciate you saying that.\"" : "\"Good. Come back then.\"";
                    }
                    break;
            }
            return "\"...\"";
        }

        /// <summary>Map a personality name to a broad approach cluster used to select pickup lines.</summary>
        private string GetApproachCluster(string personality)
        {
            if (personality == null) return "playful";
            switch (personality)
            {
                case "Shy": case "Sweet": case "Romantic": case "Needy":
                    return "gentle";
                case "Dominant": case "Aggressive": case "Street Smart": case "Independent":
                    return "confident";
                case "Cold": case "Sarcastic": case "Mysterious": case "Classy":
                    return "cool";
                case "Gold Digger": case "Manipulative": case "Jealous": case "Unstable":
                    return "flattering";
                default: // Flirty, Party Girl, Playful, Chaotic
                    return "playful";
            }
        }

        /// <summary>
        /// Map a selected menu index back to its cluster name.
        /// First encounter uses groups of 4 (gentle=0-3, confident=4-7, cool=8-11, flattering=12-15, playful=16-19).
        /// Returning encounter uses groups of 3 (gentle=0-2, confident=3-5, cool=6-8, flattering=9-11, playful=12-14).
        /// </summary>
        private string GetClusterFromIndex(int idx, bool firstEncounter)
        {
            string[] clusters = { "gentle", "confident", "cool", "flattering", "playful" };
            int groupSize = firstEncounter ? 4 : 3;
            int group = idx / groupSize;
            return (group >= 0 && group < clusters.Length) ? clusters[group] : null;
        }
    }
}
