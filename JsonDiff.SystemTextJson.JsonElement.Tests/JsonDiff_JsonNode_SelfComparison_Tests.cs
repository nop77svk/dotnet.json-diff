#if NET8_0_OR_GREATER
namespace NoP77svk.JsonDiff.SystemTextJson.Tests;

using System.Text.Json;
using System.Text.Json.Nodes;

using NoP77svk.JsonDiff;

public class JsonDiff_JsonNode_Tests
{
    private readonly SimpleJsonDiffFormatter<JsonNode?> _jsonNodeDiffFormatter = new();

    private readonly JsonDocumentOptions _jsonDocumentParseOptions = new()
    {
        AllowTrailingCommas = true,
        CommentHandling = JsonCommentHandling.Skip,
#if NET10_0_OR_GREATER
        AllowDuplicateProperties = true,
#endif
        MaxDepth = 20
    };

    private readonly JsonNodeOptions _jsonNodeParseOptions = new()
    {
        PropertyNameCaseInsensitive = false
    };

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task Basic_JsonNode_SelfComparison_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        Assert.That(jsonDocument, Is.Not.Null);

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonDocument?.CompareWith(jsonDocument)
            ?? throw new InvalidOperationException("JsonNode parsing resulted in null");

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonNodeDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task JsonNode_SelfComparison_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new();

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonNodeDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task JsonNode_SelfComparisonByPosition_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonNodeDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task JsonNode_SelfComparisonByKeyAndPosition_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonNodeDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task JsonNode_SelfComparisonByPositionAndName_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonNodeDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.BasicTestCases))]
    public async Task JsonNode_SelfComparisonByKeyAndName_ReturnsEmpty(string testCaseFileName)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Test case file: {testCaseFileName}");

        using Stream testCaseStream = File.OpenRead(testCaseFileName);
        JsonNode? jsonDocument = await JsonNode.ParseAsync(testCaseStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);
        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(jsonDocument, jsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonNodeDiffFormatter));
    }
}
#endif
