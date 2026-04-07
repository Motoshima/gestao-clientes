using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using BCrypt.Net;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly string _connectionString;

    public UsuarioController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

   
    // 🔹 POST
    [HttpPost]
public IActionResult InserirUsuario([FromBody] Usuarios user)
{
    string senhaHash = BCrypt.Net.BCrypt.HashPassword(user.Senha);

    using (SqlConnection conn = new SqlConnection(_connectionString))
    {
        conn.Open();

        string checkQuery = "SELECT COUNT (*) FROM usuarios WHERE email = @email";
        using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
        {
            checkCmd.Parameters.AddWithValue("@email", user.Email);

            int count = (int)checkCmd.ExecuteScalar();
            if (count > 0)
            {
                return BadRequest("Email já cadastrado.");
            }
        }

        string InsertQuery = "INSERT INTO usuarios (nome, email, nascimento, senha, cpf) VALUES (@nome, @email, @nascimento, @senha, @cpf)";

        using (SqlCommand cmd = new SqlCommand(InsertQuery, conn))
        {
            cmd.Parameters.AddWithValue("@nome", user.Nome);
            cmd.Parameters.AddWithValue("@email", user.Email);

            DateTime dataConvertida = DateTime.Parse(user.DataNasc);
            cmd.Parameters.Add("@nascimento", SqlDbType.DateTime).Value = dataConvertida;

            cmd.Parameters.AddWithValue("@senha", senhaHash);
            cmd.Parameters.AddWithValue("@cpf", user.cpf);

            cmd.ExecuteNonQuery();
        }
    }

    return Ok("Usuário Criado com sucesso");
}
}