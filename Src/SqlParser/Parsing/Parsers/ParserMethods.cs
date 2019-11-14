using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlParser.Parsing.Parsers
{
    public static class ParserMethods
    {
        /// <summary>
        /// Parse the first time and then attempt to parse the right rule. If the right rule cannot be parsed
        /// the result of the left item is returned. This is used when we only want to parse the left side
        /// of a postfix production once without backtracking
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="left"></param>
        /// <param name="getRight"></param>
        /// <returns></returns>
        public static IParser<TInput, TOutput> Apply<TInput, TOutput>(IParser<TInput, TOutput> left, Func<IParser<TInput, TOutput>, IParser<TInput, TOutput>> getRight)
            => new ApplyParser<TInput, TOutput>(left, getRight);

        public static IParser<char, string> Characters(string pattern)
            => new MatchSequenceParser<char, string>(pattern, c => pattern);

        public static IParser<char, TOutput> Characters<TOutput>(string pattern, Func<char[], TOutput> produce)
            => new MatchSequenceParser<char, TOutput>(pattern, produce);

        public static IParser<TInput, TOutput> Deferred<TInput, TOutput>(Func<IParser<TInput, TOutput>> getParser)
            => new DeferredParser<TInput, TOutput>(getParser);

        public static IParser<TInput, TOutput> Error<TInput, TOutput>(Func<ISequence<TInput>, string> getError)
            => Produce<TInput, TOutput>(t => throw new Exception(getError(t)));

        public static IParser<TInput, TOutput> First<TInput, TOutput>(params IParser<TInput, TOutput>[] parsers)
            => new FirstParser<TInput, TOutput>(parsers.ToArray());

        public static IParser<TInput, TOutput> List<TInput, TItem, TOutput>(IParser<TInput, TItem> parser, Func<IReadOnlyList<TItem>, TOutput> produce, bool atLeastOne = false)
            => new ListParser<TInput, TItem, TOutput>(parser, produce, atLeastOne);

        public static IParser<T, T> Match<T>(Func<T, bool> predicate)
            => new PredicateParser<T, T>(predicate, t => t);

        public static IParser<TInput, TOutput> Match<TInput, TOutput>(Func<TInput, bool> predicate, Func<TInput, TOutput> produce)
            => new PredicateParser<TInput, TOutput>(predicate, produce);

        public static IParser<TInput, TOutput> Match<TInput, TOutput>(IEnumerable<TInput> c, Func<TInput[], TOutput> produce)
            => new MatchSequenceParser<TInput, TOutput>(c, produce);

        public static IParser<TInput, TOutput> Optional<TInput, TOutput>(IParser<TInput, TOutput> parser, Func<TOutput> getDefault)
            => First(
                parser,
                Produce<TInput, TOutput>(getDefault)
            );

        public static IParser<TInput, TOutput> Produce<TInput, TOutput>(Func<TOutput> produce)
            => new ProduceParser<TInput, TOutput>(t => produce());

        public static IParser<TInput, TOutput> Produce<TInput, TOutput>(Func<ISequence<TInput>, TOutput> produce)
            => new ProduceParser<TInput, TOutput>(produce);

        public static IParser<TInput, TOutput> Require<TInput, TOutput>(IParser<TInput, TOutput> parser, Func<ISequence<TInput>, string> getErrorMessage)
            => First(
                parser,
                Error<TInput, TOutput>(getErrorMessage)
            );

        /// <summary>
        /// Parse the left side and then attempt to parse the right side. If the right side fails, the whole
        /// production fails. This is used when we only want to parse the left side once without backtracking
        /// </summary>
        /// <typeparam name="TInput"></typeparam>
        /// <typeparam name="TMiddle"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="left"></param>
        /// <param name="getRight"></param>
        /// <returns></returns>
        public static IParser<TInput, TOutput> RequireApply<TInput, TMiddle, TOutput>(IParser<TInput, TMiddle> left, Func<IParser<TInput, TMiddle>, IParser<TInput, TOutput>> getRight)
            => new ApplyParser<TInput, TMiddle, TOutput>(left, getRight);

        public static IParser<char, string> RequireCharacters(string pattern, string writeablePattern = null)
            => Require(Characters(pattern), t => $"Expected {writeablePattern ?? pattern} but found {t.Peek()}");

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, Func<T1, T2, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2 },
                (list) => produce((T1)list[0], (T2)list[1]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, Func<T1, T2, T3, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3 },
                (list) => produce((T1)list[0], (T2)list[1], (T3)list[2]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, Func<T1, T2, T3, T4, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4 },
                (list) => produce((T1)list[0], (T2)list[1], (T3)list[2], (T4)list[3]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, T5, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, IParser<TInput, T5> p5, Func<T1, T2, T3, T4, T5, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4, p5 },
                (list) => produce((T1)list[0], (T2)list[1], (T3)list[2], (T4)list[3], (T5)list[4]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, T5, T6, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, IParser<TInput, T5> p5, IParser<TInput, T6> p6, Func<T1, T2, T3, T4, T5, T6, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4, p5, p6 },
                (list) => produce((T1)list[0], (T2)list[1], (T3)list[2], (T4)list[3], (T5)list[4], (T6)list[5]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, T5, T6, T7, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, IParser<TInput, T5> p5, IParser<TInput, T6> p6, IParser<TInput, T7> p7, Func<T1, T2, T3, T4, T5, T6, T7, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4, p5, p6, p7 },
                (list) => produce((T1)list[0], (T2)list[1], (T3)list[2], (T4)list[3], (T5)list[4], (T6)list[5], (T7)list[6]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, T5, T6, T7, T8, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, IParser<TInput, T5> p5, IParser<TInput, T6> p6, IParser<TInput, T7> p7, IParser<TInput, T8> p8, Func<T1, T2, T3, T4, T5, T6, T7, T8, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4, p5, p6, p7, p8 },
                (list) => produce((T1)list[0], (T2)list[1], (T3)list[2], (T4)list[3], (T5)list[4], (T6)list[5], (T7)list[6], (T8)list[7]));
        }

        public static IParser<TInput, TOutput> Rule<TInput, T1, T2, T3, T4, T5, T6, T7, T8, T9, TOutput>(IParser<TInput, T1> p1, IParser<TInput, T2> p2, IParser<TInput, T3> p3, IParser<TInput, T4> p4, IParser<TInput, T5> p5, IParser<TInput, T6> p6, IParser<TInput, T7> p7, IParser<TInput, T8> p8, IParser<TInput, T9> p9, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TOutput> produce)
        {
            return new RuleParser<TInput, TOutput>(
                new IParser<TInput>[] { p1, p2, p3, p4, p5, p6, p7, p8, p9 },
                (list) => produce((T1)list[0], (T2)list[1], (T3)list[2], (T4)list[3], (T5)list[4], (T6)list[5], (T7)list[6], (T8)list[7], (T9)list[8]));
        }

        public static IParser<TInput, TOutput> SeparatedList<TInput, TItem, TSeparator, TOutput>(IParser<TInput, TItem> item, IParser<TInput, TSeparator> separator, Func<IReadOnlyList<TItem>, TOutput> produce, bool atLeastOne = false)
        {
            if (atLeastOne)
            {
                return Rule(
                    item,
                    List(
                        Rule(
                            separator,
                            item,
                            (s, i) => i
                        ),
                        items => items
                    ),
                    (first, rest) => produce(new[] { first }.Concat(rest).ToList())
                );
            }

            return First(
                Rule(
                    item,
                    List(
                        Rule(
                            separator,
                            item,
                            (s, i) => i
                        ),
                        items => items
                    ),
                    (first, rest) => produce(new[] { first }.Concat(rest).ToList())
                ),
                Produce<TInput, TOutput>(() => produce(new List<TItem>()))
            );
        }

        public static IParser<TInput, TTransform> Transform<TInput, TOutput, TTransform>(IParser<TInput, TOutput> parser, Func<TOutput, TTransform> produce)
            => new TransformParser<TInput, TOutput, TTransform>(parser, produce);
    }
}