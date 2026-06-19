using System.Collections;
using System.Collections.Specialized;
using AbiogenesisModel.Lib.Guard;
using AbiogenesisModel.Lib.Model;
using AbiogenesisModel.Lib.Model.Controllers;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;

namespace AbiogenesisModel.Test
{
    public class EnsureGuardTests : BaseTests
    {
        [Fact]
        public void NamedParamTest()
        {
            const string paramName = "namedNull";
            object? obj = null;
            var act = () => Ensure.ThatNamed(obj, paramName).IsNotNull();
            act.Should().Throw<ArgumentNullException>().Which.ParamName.Should().Be(paramName);
        }

        [Fact]
        public void BaseGuardsTest()
        {
            var passingActions = new List<Action>();
            var failingActions = new List<(Action action, Type exceptionType)>();

            object? objNull = null;
            var objNotNull = new object();
            var obj2 = new object();

            var dict = new Dictionary<object, int>
            {
                [objNotNull] = 42
            };

            passingActions.Add(() => Ensure.That(objNotNull).IsNotNull());
            failingActions.Add((() => Ensure.That(objNull).IsNotNull(), typeof(ArgumentNullException)));

            passingActions.Add(() => Ensure.That(objNull).IsNull());
            failingActions.Add((() => Ensure.That(objNotNull).IsNull(), typeof(ArgumentException)));

            passingActions.Add(() => Ensure.That(objNotNull).Satisfies(arg => arg == objNotNull));
            failingActions.Add((() => Ensure.That(objNotNull).Satisfies(arg => arg == objNull), typeof(ArgumentException)));

            passingActions.Add(() => Ensure.That(objNotNull).DoesNotSatisfy(arg => arg != objNotNull));
            failingActions.Add((() => Ensure.That(objNotNull).DoesNotSatisfy(arg => arg != objNull), typeof(ArgumentException)));

            passingActions.Add(() => Ensure.That(objNotNull).EqualsTo(objNotNull));
            failingActions.Add((() => Ensure.That(objNotNull).EqualsTo(obj2), typeof(ArgumentException)));

            passingActions.Add(() => Ensure.That(objNotNull).IsKeyOf(dict));
            failingActions.Add((() => Ensure.That(obj2).IsKeyOf(dict), typeof(ArgumentException)));

            passingActions.Add(() => Ensure.That(obj2).IsNotKeyOf(dict));
            failingActions.Add((() => Ensure.That(objNotNull).IsNotKeyOf(dict), typeof(ArgumentException)));

            RunActions(passingActions, failingActions);
        }

        [Fact]
        public void CollectionGuardsTest()
        {
            var passingActions = new List<Action>();
            var failingActions = new List<(Action action, Type exceptionType)>();

            IReadOnlyList<int>? col1 = null;
            IReadOnlyList<int> col2 = [];
            IReadOnlyList<object> col3 = [];
            IReadOnlyList<object> col4 = [1, 2, 3, null!];
            IReadOnlyList<int> col5 = [1, 2, 3, 4, 1];
            IReadOnlyList<object> col6 = new List<object>([1, 2]);
            var col7 = new List<int>();
            IReadOnlyList<object> col8 = [col2, col3, col2];
            var str1 = string.Empty;
            const string str2 = "!";

            passingActions.Add(() => Ensure.That(col4).IsNotEmpty());
            passingActions.Add(() => Ensure.That(col5).IsNotEmpty());
            passingActions.Add(() => Ensure.That(col6).IsNotEmpty());
            passingActions.Add(() => Ensure.That(col5.Select(i => i + 1)).IsNotEmpty());
            passingActions.Add(() => Ensure.That(str2).IsNotEmpty());
            failingActions.Add((() => Ensure.That(col1).IsNotEmpty(), typeof(ArgumentNullException)));
            failingActions.Add((() => Ensure.That(col2).IsNotEmpty(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(col3).IsNotEmpty(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(str1).IsNotEmpty(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(col7).IsNotEmpty(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(Array.Empty<int>()).IsNotEmpty(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(col7.Select(i => i + 1)).IsNotEmpty(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(new ListDictionary()).IsNotEmpty(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(new Dictionary<int, int>()).IsNotEmpty(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(new Hashtable()).IsNotEmpty(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(new HashSet<int>()).IsNotEmpty(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(col1 as List<int>).IsNotEmpty(), typeof(ArgumentNullException)));

            passingActions.Add(() => Ensure.That(col3).IsNullFree());
            passingActions.Add(() => Ensure.That(col6).IsNullFree());
            failingActions.Add((() => Ensure.That(col4).IsNullFree(), typeof(ArgumentException)));

            passingActions.Add(() => Ensure.That(col3).IsDuplicateFree());
            passingActions.Add(() => Ensure.That(col6).IsDuplicateFree());
            failingActions.Add((() => Ensure.That(col8).IsDuplicateFree(), typeof(ArgumentException)));

            RunActions(passingActions, failingActions);
        }

        [Fact]
        public void ModelDataTypesGuardsTest()
        {
            var passingActions = new List<Action>();
            var failingActions = new List<(Action action, Type exceptionType)>();

            var provider = InitServiceCollectionFromFiles();
            var strandController = provider.GetRequiredService<StrandController>();
            var bondController = provider.GetRequiredService<BondController>();
            var nucleotideController = provider.GetRequiredService<NucleotideController>();
            var moleculeController = provider.GetRequiredService<MoleculeController>();
            var stratumPopulationController = provider.GetRequiredService<StratumPopulationController>();

            var nuc1 = nucleotideController.Create(Nucleobase.A);
            var nuc2 = nucleotideController.Create(Nucleobase.U);
            var nuc3 = nucleotideController.Create(Nucleobase.A);
            var strand1 = strandController.Create(nuc1);
            var strand2 = strandController.Create(nuc2);
            var strand3 = strandController.Create(nuc3);
            var bond = bondController.Create(nuc1, nuc2);
            IReadOnlyList<Bond> bonds = [bond];
            IReadOnlyList<Bond> bonds2 = [];
            IReadOnlyList<Strand> strands = [strand1, strand2, strand3];
            IReadOnlyList<Strand> strands2 = [strand1];
            IReadOnlyList<Strand> strands3 = [strand3];
            var mol1 = moleculeController.Create(Nucleobase.C);
            var mol2 = moleculeController.Create(Nucleobase.G);
            var sp1 = stratumPopulationController.Create();
            var sp2 = stratumPopulationController.Create();
            stratumPopulationController.Add(sp1, mol1);

            passingActions.Add(() => Ensure.That(nuc1).IsBonded());
            failingActions.Add((() => Ensure.That(nuc3).IsBonded(), typeof(InvalidOperationException)));

            passingActions.Add(() => Ensure.That(nuc3).IsNotBonded());
            failingActions.Add((() => Ensure.That(nuc1).IsNotBonded(), typeof(InvalidOperationException)));

            passingActions.Add(() => Ensure.That(bonds).AllNucleotidesBelongTo(strands));
            failingActions.Add((() => Ensure.That(bonds).AllNucleotidesBelongTo(strands2), typeof(InvalidOperationException)));
            failingActions.Add((() => Ensure.That(bonds).AllNucleotidesBelongTo(strands3), typeof(InvalidOperationException)));

            passingActions.Add(() => Ensure.That(strands).AllBondsBelongTo(bonds));
            passingActions.Add(() => Ensure.That(strands2).AllBondsBelongTo(bonds));
            passingActions.Add(() => Ensure.That(strands3).AllBondsBelongTo(bonds));
            passingActions.Add(() => Ensure.That(strands3).AllBondsBelongTo(bonds2));
            failingActions.Add((() => Ensure.That(strands).AllBondsBelongTo(bonds2), typeof(InvalidOperationException)));
            failingActions.Add((() => Ensure.That(strands2).AllBondsBelongTo(bonds2), typeof(InvalidOperationException)));

            passingActions.Add(() => Ensure.That(mol1).IsIncludedIn(sp1));
            failingActions.Add((() => Ensure.That(mol2).IsIncludedIn(sp1), typeof(InvalidOperationException)));
            failingActions.Add((() => Ensure.That(mol1).IsIncludedIn(sp2), typeof(InvalidOperationException)));

            passingActions.Add(() => Ensure.That(mol1).IsNotIncludedIn(sp2));
            passingActions.Add(() => Ensure.That(mol2).IsNotIncludedIn(sp1));
            failingActions.Add((() => Ensure.That(mol1).IsNotIncludedIn(sp1), typeof(InvalidOperationException)));

            RunActions(passingActions, failingActions);
        }

        [Fact]
        public void StringGuardsTest()
        {
            var passingActions = new List<Action>();
            var failingActions = new List<(Action action, Type exceptionType)>();

            var str1 = string.Empty;
            var str2 = "";
            var str3 = " ";
            var str4 = "!";

            passingActions.Add(() => Ensure.That(str4).IsNotNullOrEmptyString());
            passingActions.Add(() => Ensure.That(str3).IsNotNullOrEmptyString());
            failingActions.Add((() => Ensure.That(str1).IsNotNullOrEmptyString(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(str2).IsNotNullOrEmptyString(), typeof(ArgumentException)));

            passingActions.Add(() => Ensure.That(str4).IsNotNullOrWhiteSpace());
            failingActions.Add((() => Ensure.That(str1).IsNotNullOrWhiteSpace(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(str2).IsNotNullOrWhiteSpace(), typeof(ArgumentException)));
            failingActions.Add((() => Ensure.That(str3).IsNotNullOrWhiteSpace(), typeof(ArgumentException)));

            RunActions(passingActions, failingActions);
        }

        [Fact]
        public void ReferenceGuardsTest()
        {
            var passingActions = new List<Action>();
            var failingActions = new List<(Action action, Type exceptionType)>();

            var obj1 = new object();
            var obj2 = new object();
            var obj3 = obj2;

            passingActions.Add(() => Ensure.That(obj1).IsNotSameAs(obj2));
            failingActions.Add((() => Ensure.That(obj2).IsNotSameAs(obj3), typeof(ArgumentException)));

            RunActions(passingActions, failingActions);
        }

        [Fact]
        public void ComparableGuardsTest()
        {
            var passingActions = new List<Action>();
            var failingActions = new List<(Action action, Type exceptionType)>();

            IReadOnlyList<int> col1 = [1, 2, 3];
            IReadOnlyList<int> col2 = [1, 2, 3, 4, 1];
            var index = 4;

            passingActions.Add(() => Ensure.That(index).IsGreaterThan(3));
            failingActions.Add((() => Ensure.That(index).IsGreaterThan(index), typeof(ArgumentOutOfRangeException)));

            passingActions.Add(() => Ensure.That(index).IsGreaterOrEqual(3));
            passingActions.Add(() => Ensure.That(index).IsGreaterOrEqual(index));
            failingActions.Add((() => Ensure.That(index).IsGreaterOrEqual(10), typeof(ArgumentOutOfRangeException)));

            passingActions.Add(() => Ensure.That(index).IsLessThan(30));
            failingActions.Add((() => Ensure.That(index).IsLessThan(index), typeof(ArgumentOutOfRangeException)));

            passingActions.Add(() => Ensure.That(index).IsLessOrEqual(30));
            passingActions.Add(() => Ensure.That(index).IsLessOrEqual(index));
            failingActions.Add((() => Ensure.That(index).IsLessOrEqual(1), typeof(ArgumentOutOfRangeException)));

            passingActions.Add(() => Ensure.That(index).IsInRange(1, 5));
            failingActions.Add((() => Ensure.That(index).IsInRange(1, 2), typeof(ArgumentOutOfRangeException)));

            passingActions.Add(() => Ensure.That(index).IsInRangeOfIndexes(col2));
            failingActions.Add((() => Ensure.That(index).IsInRangeOfIndexes(col1), typeof(ArgumentOutOfRangeException)));

            RunActions(passingActions, failingActions);
        }

        private static void RunActions(List<Action> passingActions, List<(Action action, Type exceptionType)> failingActions)
        {
            using (new AssertionScope())
            {
                foreach (var action in passingActions)
                {
                    action.Should().NotThrow();
                }

                foreach (var (action, exceptionType) in failingActions)
                {
                    action.Should().Throw().Which.Should().BeOfType(exceptionType);
                }
            }
        }
    }
}
