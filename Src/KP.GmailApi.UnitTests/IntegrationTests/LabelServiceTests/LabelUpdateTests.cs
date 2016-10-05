﻿using System;
using System.Threading.Tasks;
using KP.GmailApi.Common;
using KP.GmailApi.Models;
using KP.GmailApi.Services;
using Xunit;

namespace KP.GmailApi.UnitTests.IntegrationTests.LabelServiceTests
{
    public class LabelUpdateTests : IDisposable
    {
        private const string TestLabel = "Testing/";
        private readonly LabelService _service;
        private readonly CleanupHelper<Label, CreateLabelInput> _helper;

        public LabelUpdateTests()
        {
            GmailProxy proxy = SettingsManager.GetGmailProxy();
            _service = new LabelService(proxy);

            Func<Label, Task> deleteAction = label => _service.DeleteAsync(label.Id);
            Func<CreateLabelInput, Task<Label>> createAction = async input => await _service.CreateAsync(input);
            _helper = new CleanupHelper<Label, CreateLabelInput>(createAction, deleteAction);
        }

        [Fact]
        public async Task CanUpdate()
        {
            // Arrange
            var random = new Random();
            Label createdLabel = await _helper.CreateAsync(new CreateLabelInput(TestLabel + random.Next()));
            string newName = TestLabel + random.Next();

            // Act
            Label label = await _service.UpdateAsync(new UpdateLabelInput(createdLabel.Id) { Name = newName });

            // Assert
            Assert.NotNull(label);
            Assert.Equal(createdLabel.Id, label.Id);
            Assert.Equal(newName, label.Name);
        }

        public void Dispose()
        {
            _helper.Cleanup();
        }
    }
}
