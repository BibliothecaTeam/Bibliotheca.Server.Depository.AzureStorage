using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bibliotheca.Server.Depository.Abstractions.DataTransferObjects;
using Bibliotheca.Server.Depository.Abstractions.Exceptions;
using Bibliotheca.Server.Depository.AzureStorage.Core.Validators;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Bibliotheca.Server.Depository.AzureStorage.Core.Services
{
    public class ProjectsService : IProjectsService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly ICommonValidator _commonValidator;
        private readonly ILogger _logger;

        public ProjectsService(IFileSystemService fileSystemService, ICommonValidator commonValidator, ILoggerFactory loggerFactory)
        {
            _fileSystemService = fileSystemService;
            _commonValidator = commonValidator;
            _logger = loggerFactory.CreateLogger<ProjectsService>();
        }

        public async Task<IList<ProjectDto>> GetProjectsAsync()
        {
            var projectIds = await _fileSystemService.GetProjectsIdsAsync();
            projectIds = projectIds.OrderBy(x => x).ToList();

            var projectDtos = new List<ProjectDto>();
            foreach (var projectId in projectIds)
            {
                try
                {
                    var configurationFile = await _fileSystemService.ReadTextAsync(projectId, "configuration.json");
                    var projectDto = JsonConvert.DeserializeObject<ProjectDto>(configurationFile);

                    projectDto.Id = projectId;
                    projectDtos.Add(projectDto);
                }
                catch (FileNotFoundException)
                {
                    _logger.LogWarning($"Project '{projectId}' doesn't have configuration file.");
                }
            }

            return projectDtos;
        }

        public async Task<ProjectDto> GetProjectAsync(string projectId)
        {
            await _commonValidator.ProjectHaveToExists(projectId);

            try
            {
                var configurationFile = await _fileSystemService.ReadTextAsync(projectId, "configuration.json");
                var projectDto = JsonConvert.DeserializeObject<ProjectDto>(configurationFile);

                projectDto.Id = projectId;
                return projectDto;
            }
            catch (FileNotFoundException)
            {
                _logger.LogWarning($"Project '{projectId}' doesn't have configuration file.");
                throw new ConfigurationFileNotFoundException($"Configuration file for project '{projectId}' not found.");
            }
        }

        public async Task CreateProjectAsync(ProjectDto project)
        {
            _commonValidator.ProjectIdShouldBeSpecified(project.Id);
            await _commonValidator.ProjectShouldNotExists(project.Id);

            await _fileSystemService.CreateFolderAsync(project.Id);

            var serializedProject = JsonConvert.SerializeObject(project);
            await _fileSystemService.WriteTextAsync(project.Id, "configuration.json", serializedProject);
        }

        public async Task UpdateProjectAsync(string projectId, ProjectDto project)
        {
            await _commonValidator.ProjectHaveToExists(projectId);

            project.Id = projectId;
            var serializedProject = JsonConvert.SerializeObject(project);
            await _fileSystemService.WriteTextAsync(projectId, "configuration.json", serializedProject);
        }

        public async Task DeleteProjectAsync(string projectId)
        {
            await _commonValidator.ProjectHaveToExists(projectId);
            await _fileSystemService.DeleteFolderAsync(projectId);
        }
    }
}