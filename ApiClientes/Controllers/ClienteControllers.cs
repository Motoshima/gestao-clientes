using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly string _connectionString;

    public ClientesController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    // 🔹 POST
    [HttpPost]
    public IActionResult InserirCliente([FromBody] Cliente cliente)
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            string query = "INSERT INTO clientes (nome, email) VALUES (@nome, @email)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@nome", cliente.Nome);
                cmd.Parameters.AddWithValue("@email", cliente.Email);

                cmd.ExecuteNonQuery();
            }
        }

        return Ok("Cliente inserido com sucesso");
    }

    // 🔥 GET (AGORA NO LUGAR CERTO)
    [HttpGet]
    public IActionResult ListarClientes()
    {
        List<Cliente> lista = new List<Cliente>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();

            string query = "SELECT * FROM clientes";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Cliente c = new Cliente
                        {
                            Id = (int)reader["id"],
                            Nome = reader["nome"].ToString(),
                            Email = reader["email"].ToString()
                        };

                        lista.Add(c);
                    }
                }
            }
        }

        return Ok(lista);
    }

[HttpGet("buscar")]
public IActionResult BuscarPorNome(string nome)
{
    List<Cliente> lista = new List<Cliente>();

    using (SqlConnection conn = new SqlConnection(_connectionString))
    {
        conn.Open();

        string query = "SELECT * FROM clientes WHERE nome LIKE @nome";

        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@nome", "%" + nome + "%");

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Cliente c = new Cliente
                    {
                        Id = (int)reader["id"],
                        Nome = reader["nome"].ToString(),
                        Email = reader["email"].ToString()
                    };

                    lista.Add(c);
                }
            }
        }
    }

    return Ok(lista);
}

[HttpDelete("{id}")]
public IActionResult DeletarCliente(int id)
{
    using (SqlConnection conn = new SqlConnection(_connectionString))
    {
        conn.Open();

        string query = "DELETE FROM clientes WHERE id = @id";

        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@id", id);

            int linhasAfetadas = cmd.ExecuteNonQuery();

            if (linhasAfetadas == 0)
            {
                return NotFound("Cliente não encontrado");
            }
        }
    }

    return Ok("Cliente deletado com sucesso");
}
[HttpPut("{id}")]
public IActionResult AtualizarCliente(int id, [FromBody] Cliente cliente)
{
    using (SqlConnection conn = new SqlConnection(_connectionString))
    {
        conn.Open();

        string query = "UPDATE clientes SET nome = @nome, email = @email WHERE id = @id";

        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
            cmd.Parameters.AddWithValue("@nome", cliente.Nome);
            cmd.Parameters.AddWithValue("@email", cliente.Email);
            cmd.Parameters.AddWithValue("@id", id);

            int linhasAfetadas = cmd.ExecuteNonQuery();

            if (linhasAfetadas == 0)
            {
                return NotFound("Cliente não encontrado");
            }
        }
    }

    return Ok("Cliente atualizado com sucesso");
}
}