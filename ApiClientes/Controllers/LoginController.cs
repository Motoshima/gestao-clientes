using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace LoginController.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.cpf) || string.IsNullOrEmpty(request.senha))
                return BadRequest("Usuário e senha são obrigatórios.");

            string query = @"
                SELECT Id, cpf, senha
                FROM usuarios
                WHERE cpf = @cpf";

            using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@cpf", request.cpf);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return Unauthorized("Usuário não encontrado.");

                    string storedHash = reader["senha"].ToString();

                    bool senhaValida = BCrypt.Net.BCrypt.Verify(request.senha, storedHash);

                    if (!senhaValida)
                        return Unauthorized("Senha incorreta.");

                    int userId = (int)reader["Id"];

                    string token = GenerateToken(userId, request.cpf);

                    return Ok(new { Token = token });
                }
            }
        }

        // Token fake (depois a gente troca por JWT)
        private string GenerateToken(int userId, string username)
        {
            return $"fake-token-{userId}";
        }
    }
}