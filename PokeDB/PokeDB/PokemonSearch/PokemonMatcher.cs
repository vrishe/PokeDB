using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using PokeDB.GameData;

namespace PokeDB.PokemonSearch
{
    class PokemonMatcher
    {
        interface IMatcher
        {
            bool Match(Pokemon pokemon, string[] query, ref int i);
        }

        readonly char[] querySplitChars = new[] { ' ' };
        readonly IMatcher[] matchers = new IMatcher[]
        {
            new StatMatcher(),
            new TypeMatcher(),
            new NameMatcher()
        };


        public Func<Pokemon, bool> Match(string query)
        {
#if DEBUG
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
#endif // DEBUG

            var queryParts = query.Split(querySplitChars, StringSplitOptions.RemoveEmptyEntries);

            if (queryParts.Length > 0)
            {
                return p =>
                {
                    for (int i = 0; i < queryParts.Length;)
                    {
                        bool result = false;

                        foreach (var matcher in matchers)
                        {
                            if (i < queryParts.Length)
                            {
                                if (matcher.Match(p, queryParts, ref i))
                                {
                                    result = true;

                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (!result)
                        {
                            DebugLog(() => $"{nameof(PokemonMatcher)}: {p.Name} Rejected.");

                            return false;
                        }
                    }
                    DebugLog(() => $"{nameof(PokemonMatcher)}: {p.Name} Matched!");

                    return true;
                };
            }
            return p => true;
        }

        public bool IsMatch(Pokemon pokemon, string query)
        {
            return Match(query)(pokemon);
        }


        class StatMatcher : IMatcher
        {
            readonly IDictionary<char, Func<Pokemon, double, bool>> statMatchers
                = new Dictionary<char, Func<Pokemon, double, bool>>
                {
                    { 'a', new Func<Pokemon, double, bool>((p, v) => StatEquals(p.Attack, v)) },
                    { 'd', new Func<Pokemon, double, bool>((p, v) => StatEquals(p.Defense, v)) },
                    { 's', new Func<Pokemon, double, bool>((p, v) => StatEquals(p.Stamina, v)) },
                };

            static bool StatEquals(double current, double expected)
            {
                return current.Equals(expected)
                    || (((long)Math.Round(current)) == ((long)Math.Round(expected)));
            }


            readonly NumberStyles numberStyle = NumberStyles.Integer | NumberStyles.AllowDecimalPoint;

            public bool Match(Pokemon pokemon, string[] query, ref int i)
            {
                var token = query[i];

                DebugLog(() => $"{nameof(StatMatcher)}: {pokemon.Name} ({pokemon.Attack}/{pokemon.Defense}/{pokemon.Stamina}) matching stats against {token}...");

                var match = Regex.Match(token, @"^(?<stat>a(tk)?|d(ef)?|s(ta)?)(?::(?<val>\d+)?)?$",
                    RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                if (!match.Success)
                {
                    DebugLog(() => $"{nameof(StatMatcher)}: [Fail Fast] Can't match {token} -> FAILED.");

                    return false;
                }
                DebugLog(() => $"{nameof(StatMatcher)}: - matched {token} for stat {match.Groups["stat"].Value}.");

                int increment = 1;

                double value;
                {
                    var val = match.Groups["val"];

                    if (val.Success)
                    {
                        DebugLog(() => $"{nameof(StatMatcher)}: - there is A value {match.Groups["val"].Value}.");

                        value = double.Parse(val.Value, numberStyle, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        DebugLog(() => $"{nameof(StatMatcher)}: - there is NO value. Need to take next token.");

                        if (i >= query.Length - 1)
                        {
                            DebugLog(() => $"{nameof(StatMatcher)}: [Fail Fast] No tokens within query -> FAILED.");

                            return false;
                        }
                        var valueRaw = query[i + 1].Trim("!?:.,".ToCharArray());

                        if (!double.TryParse(valueRaw, numberStyle, CultureInfo.InvariantCulture, out value))
                        {
                            DebugLog(() => $"{nameof(StatMatcher)}: [Fail Fast] Can't parse next query token as double -> FAILED.");

                            return false;
                        }
                        ++increment;
                    }
                }
                if (statMatchers[char.ToLowerInvariant(match.Groups["stat"].Value[0])](pokemon, value))
                {
                    i += increment;

                    DebugLog(() => $"{nameof(StatMatcher)}: Matching {pokemon.Name} -> SUCCEEDED.");

                    return true;
                }
                DebugLog(() => $"{nameof(StatMatcher)}: Matching {pokemon.Name} -> FAILED.");

                return false;
            }
        }

        class TypeMatcher : IMatcher
        {
            public bool Match(Pokemon pokemon, string[] query, ref int i)
            {
                var jmax = Math.Min(pokemon.Types.Length, query.Length - i);

                DebugLog(() => $"{nameof(TypeMatcher)}: {pokemon.Name} ({string.Join("/", pokemon.Types.Select(type => type.Name))}) matching {jmax} types...");

                for (int j = 0; j < jmax; ++j)
                {
                    var token = query[i + j];

                    DebugLog(() => $"{nameof(TypeMatcher)}: - picked {token}");

                    if (!pokemon.Types[j].Name
                        .Equals(token, StringComparison.OrdinalIgnoreCase))
                    {
                        DebugLog(() => $"{nameof(TypeMatcher)}: [Fail Fast] {pokemon.Name} has {pokemon.Types[j].Name}, but {token} expected -> FAILED.");

                        return false;
                    }
                    DebugLog(() => $"{nameof(TypeMatcher)}: - {token} OK");
                }
                i += jmax;

                DebugLog(() => $"{nameof(TypeMatcher)}: Matching {pokemon.Name} -> SUCCEEDED.");

                return true;
            }
        }

        class NameMatcher : IMatcher
        {
            public bool Match(Pokemon pokemon, string[] query, ref int i)
            {
                var token = query[i];

                DebugLog(() => $"{nameof(TypeMatcher)}: {pokemon.Name} matching name against {token}...");

                if (!pokemon.Name.StartsWith(query[i], StringComparison.OrdinalIgnoreCase))
                {
                    DebugLog(() => $"{nameof(TypeMatcher)}: Matching {pokemon.Name} -> FAILED.");

                    return false;
                }
                ++i;

                DebugLog(() => $"{nameof(TypeMatcher)}: Matching {pokemon.Name} -> SUCCEEDED.");

                return true;
            }
        }


#if DEBUG
        static bool debuggable;

        [Conditional("DEBUG")]
        public static void EnableDebug()
        {
            debuggable = true;
        }

        [Conditional("DEBUG")]
        static void DebugLog(Func<string> outputBuilder)
        {
            if (debuggable)
            {
                Debug.WriteLine(outputBuilder?.Invoke() ?? string.Empty);
            }
        }
#else
        [Conditional("DEBUG")]
        public static void EnableDebug()
        {
            /* Nothing to do */
        }

        [Conditional("DEBUG")]
        static void DebugLog(Func<string> outputBuilder)
        {
            /* Nothing to do */
        }
#endif // DEBUG
    }
}
