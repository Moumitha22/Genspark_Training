using BankManagementSystem.Interfaces;
using BankManagementSystem.Models;
using BankManagementSystem.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BankManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<ActionResult<Account>> CreateAccount([FromBody] AccountAddRequestDto accountDto)
        {
            try
            {
                var createdAccount = await _accountService.CreateAccount(accountDto);
                return Ok(createdAccount);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult<ICollection<Account>>> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccounts();
                return Ok(accounts);
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {
            try
            {
                var account = await _accountService.GetAccountById(id);
                return Ok(account);
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("accountNumber/{accountNumber}")]
        public async Task<ActionResult<Account>> GetAccountByAccountNumber(string accountNumber)
        {
            try
            {
                var account = await _accountService.GetAccountByAccountNumber(accountNumber);
                return Ok(account);
            }
            catch (System.Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("check-balance/{accountNumber}")]
        public async Task<ActionResult<decimal>> CheckBalance(string accountNumber)
        {
            try
            {
                var balance = await _accountService.CheckBalance(accountNumber);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("close/{accountNumber}")]
        public async Task<IActionResult> CloseAccount(string accountNumber)
        {
            try
            {
                var result = await _accountService.CloseAccount(accountNumber);
                return Ok(new { Message = "Account deactivated successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
