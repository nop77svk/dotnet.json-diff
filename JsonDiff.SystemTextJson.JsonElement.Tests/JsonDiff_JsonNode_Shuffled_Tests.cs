#if NET8_0_OR_GREATER
namespace NoP77svk.JsonDiff.SystemTextJson.Tests;

using System.Text.Json;
using System.Text.Json.Nodes;

public class JsonDiff_JsonNode_ShuffledKvp_Tests
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
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
    public async Task JsonNode_ShuffledComparisonByKeyAndName_ReturnsEmpty(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument, shuffledJsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonNodeDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
    public async Task JsonNode_ShuffledComparisonByPositionAndName_ReturnsEmpty(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument, shuffledJsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonNodeDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
    public async Task JsonNode_ShuffledComparisonByKeyAndPosition_ReturnsDifferences(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument, shuffledJsonDocument);

        // assert
        Assert.That(differences, Is.Not.Empty);
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledTestCases))]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
    public async Task JsonNode_ShuffledComparisonByPositionAndPosition_ReturnsDifferences(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Position
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument, shuffledJsonDocument);

        // assert
        Assert.That(differences, Is.Not.Empty);
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
    public async Task JsonNode_ShuffledKvpComparisonByKeyAndName_ReturnsEmpty(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        IJsonDiffNodeValuesSelector<JsonNode?> kvpMatchingNodeValuesSelector = new JsonNodeDiffValuesSelector()
        {
            ArrayElementDescriptorSelector = (index, elementNode) =>
            {
                if (elementNode is not JsonObject element)
                {
                    return $"element #{index}";
                }

                string? result = null;
                if (element.TryGetPropertyValue("key", out JsonNode? keyProperty)
                    || element.TryGetPropertyValue("id", out keyProperty)
                    || element.TryGetPropertyValue("type", out keyProperty)
                    || element.TryGetPropertyValue("org_id", out keyProperty)
                    || element.TryGetPropertyValue("user_id", out keyProperty)
                    || element.TryGetPropertyValue("widget_id", out keyProperty))
                {
                    result = keyProperty?.GetValueKind() is JsonValueKind.String or JsonValueKind.Number
                        ? keyProperty.GetValue<string>()
                        : null;
                }

                return result ?? $"element #{index}";
            }
        };

        JsonComparer<JsonNode?> jsonComparer = new(kvpMatchingNodeValuesSelector)
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Key,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument, shuffledJsonDocument);

        // assert
        Assert.That(differences, Is.Empty, () => JsonDiff_Data.DifferencesToString(differences, _jsonNodeDiffFormatter));
    }

    [Test]
    [TestCaseSource(typeof(JsonDiff_Data), nameof(JsonDiff_Data.ShuffledKvpTestCases))]
    public async Task JsonNode_ShuffledKvpComparisonByPositionAndName_ReturnsDifferences(ShuffledJsonTestCase testCase)
    {
        // arrange
        await TestContext.Out.WriteLineAsync($"Original file: {testCase.OriginalFileName}");
        await TestContext.Out.WriteLineAsync($"Shuffled file: {testCase.ShuffledFileName}");

        using Stream originalJsonStream = File.OpenRead(testCase.OriginalFileName);
        JsonNode? originalJsonDocument = await JsonNode.ParseAsync(originalJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        using Stream shuffledJsonStream = File.OpenRead(testCase.ShuffledFileName);
        JsonNode? shuffledJsonDocument = await JsonNode.ParseAsync(shuffledJsonStream, _jsonNodeParseOptions, _jsonDocumentParseOptions);

        JsonNodeComparer jsonComparer = new()
        {
            ArrayElementMatchingStrategy = MatchJsonArrayElementsBy.Position,
            ObjectPropertiesMatchingStrategy = MatchJsonObjectPropertiesBy.Name
        };

        // act
        IEnumerable<JsonDifference<JsonNode?>> differences = jsonComparer.EnumerateDifferences(originalJsonDocument, shuffledJsonDocument);

        // assert
        Assert.That(differences, Is.Not.Empty);
    }
}
#endif
