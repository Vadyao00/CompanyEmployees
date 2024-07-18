using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
        {
            _repositoryManager = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
        {
            var company = _repositoryManager.Company.GetCompany(companyId, trackChanges);
            if(company is null)
                throw new CompanyNotFoundException(companyId);

            var employeesFromDb = _repositoryManager.Employee.GetEmployees(companyId, trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

            return employeesDto;
        }

        public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
        {
            var company = _repositoryManager.Company.GetCompany(companyId,trackChanges);
            if(company is null)
                throw new CompanyNotFoundException(companyId);

            var employeeDb = _repositoryManager.Employee.GetEmployee(companyId, id, trackChanges);
            if(employeeDb is null)
                throw new EmployeeNotFoundException(id);

            var employeeDto = _mapper.Map<EmployeeDto>(employeeDb);

            return employeeDto;
        }

        public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
        {
            var company = _repositoryManager.Company.GetCompany(companyId, trackChanges);
            if(company is null)
                throw new CompanyNotFoundException(companyId);

            var employee = _mapper.Map<Employee>(employeeForCreation);

            _repositoryManager.Employee.CreateEmployeeForCompany(companyId, employee);
            _repositoryManager.Save();

            var employeeToReturn = _mapper.Map<EmployeeDto>(employee);
            return employeeToReturn;
        }

        public void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges)
        {
            var company = _repositoryManager.Company.GetCompany(companyId, trackChanges);
            if( company is null)
                throw new CompanyNotFoundException(companyId);

            var employeeForCompany = _repositoryManager.Employee.GetEmployee(companyId, id, trackChanges);
            if(employeeForCompany is null)
                throw new EmployeeNotFoundException(companyId);

            _repositoryManager.Employee.DeleteEmployee(employeeForCompany);
            _repositoryManager.Save();
        }

        public void UpdateEmployeeForCompany(Guid companyId, Guid Id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
        {
            var company = _repositoryManager.Company.GetCompany(companyId, compTrackChanges);
            if(company is null)
                throw new CompanyNotFoundException(companyId);

            var employeeEntity = _repositoryManager.Employee.GetEmployee(companyId, Id, empTrackChanges);
            if( employeeEntity is null)
                throw new EmployeeNotFoundException(companyId);

            _mapper.Map(employeeForUpdate, employeeEntity);
            _repositoryManager.Save();
        }
    }
}
