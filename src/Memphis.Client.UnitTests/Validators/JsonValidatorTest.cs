using Memphis.Client.Constants;
using Memphis.Client.Exception;
using Memphis.Client.UnitTests.Validators.TestData;
using Memphis.Client.Validators;

namespace Memphis.Client.UnitTests.Validators
{
    public class JsonValidatorTest
    {
        private readonly ISchemaValidator _sut;

        public JsonValidatorTest()
        {
            _sut = new JsonValidator();
        }

        #region JsonValidatorTest.ParseAndStore
        [Theory]
        [MemberData(nameof(JsonValidatorTestData.ValidSchema), MemberType = typeof(JsonValidatorTestData))]
        public void ShouldReturnTrue_WhenParseAndStore_WhereValidSchemaPassed(string validSchema)
        {
            var schemaUpdate = ValidatorTestHelper.GetSchemaUpdateInit(
                "test-schema-001",
                validSchema,
                MemphisSchemaTypes.JSON
            );

            var actual = _sut.AddOrUpdateSchema(schemaUpdate);

            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(JsonValidatorTestData.InvalidSchema), MemberType = typeof(JsonValidatorTestData))]
        public void ShouldReturnFalse_WhenParseAndStore_WhereInvalidSchemaPassed(string invalidSchema)
        {
            var schemaUpdate = ValidatorTestHelper.GetSchemaUpdateInit(
                "test-schema-001",
                invalidSchema,
                MemphisSchemaTypes.JSON
            );

            var actual = _sut.AddOrUpdateSchema(schemaUpdate);

            Assert.False(actual);
        }

        #endregion


        #region JsonValidatorTest.ValidateAsync

        [Theory]
        [MemberData(nameof(JsonValidatorTestData.ValidSchemaDetail), MemberType = typeof(JsonValidatorTestData))]
        public async Task ShouldDoSuccess_WhenValidateAsync_WhereValidDataPassed(string schemaKey, string schema, byte[] msg)
        {
            var schemaUpdate = ValidatorTestHelper.GetSchemaUpdateInit(
                schemaKey,
                schema,
                MemphisSchemaTypes.JSON
            );

            _sut.AddOrUpdateSchema(schemaUpdate);

            await _sut.ValidateAsync(msg, schemaKey);
        }

        [Theory]
        [MemberData(nameof(JsonValidatorTestData.InvalidSchemaDetail), MemberType = typeof(JsonValidatorTestData))]
        public async Task ShouldDoThrow_WhenValidateAsync_WhereInvalidDataPassed(string schemaKey, string schema, byte[] msg)
        {
            var schemaUpdate = ValidatorTestHelper.GetSchemaUpdateInit(
                schemaKey,
                schema,
                MemphisSchemaTypes.JSON
            );

            _sut.AddOrUpdateSchema(schemaUpdate);

            await Assert.ThrowsAsync<MemphisSchemaValidationException>(
                 () => _sut.ValidateAsync(msg, schemaKey));
        }

        [Theory]
        [MemberData(nameof(JsonValidatorTestData.Message), MemberType = typeof(JsonValidatorTestData))]
        public async Task ShouldDoThrow_WhenValidateAsync_WhereSchemaNotFoundInCache(byte[] msg)
        {
            var nonexistentSchema = Guid.NewGuid().ToString();

            await Assert.ThrowsAsync<MemphisSchemaValidationException>(
                () => _sut.ValidateAsync(msg, nonexistentSchema));
        }

        #endregion
    }
}