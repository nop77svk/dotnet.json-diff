namespace NoP77svk.JsonDiff.Tests;

using System.Text;
using System.Text.Json;

using NoP77svk.JsonDiff;

[TestFixture]
public class JsonDiff_JsonElement_Tests
{
    private readonly JsonDocumentOptions _jsonDocumentParseOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
#if NET10_0_OR_GREATER
        AllowDuplicateProperties = true,
#endif
        MaxDepth = 20
    };

    private readonly SimpleJsonDiffFormatter<JsonElement> _jsonElementDiffFormatter = new();

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task Basic_JsonElement_SelfComparison_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonDocument.CompareWith(jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonElementDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task JsonElement_SelfComparison_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new();

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonElementDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task JsonElement_SelfComparisonByPosition_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonElementDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task JsonElement_SelfComparisonByKeyAndPosition_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonElementDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task JsonElement_SelfComparisonByPositionAndName_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonElementDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task JsonElement_SelfComparisonByKeyAndName_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        using JsonDocument jsonDocument = await JsonDocument.ParseAsync(testCaseStream, _jsonDocumentParseOptions);
        JsonElementComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonElement>> differences = jsonComparer.EnumerateDifferences(jsonDocument.RootElement, jsonDocument.RootElement);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonElementDiffFormatter));
    }
}
