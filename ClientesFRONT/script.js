const api = "http://localhost:5107/api/clientes";

// 🔍 BUSCAR
function buscar() {
    const nome = document.getElementById("busca").value;

    // 🔥 MOSTRA A TABELA
    document.getElementById("listaCard").classList.remove("d-none");
    document.getElementById("containerPrincipal").classList.add("mostrar-lista");

    fetch(`${api}/buscar?nome=${nome}`)
        .then(res => res.json())
        .then(data => {
            const lista = document.getElementById("lista");
            lista.innerHTML = "";

            data.forEach(c => {
                const tr = document.createElement("tr");

                tr.innerHTML = `
                    <td>${c.nome}</td>
                    <td>${c.email}</td>
                    <td>
                        <button onclick="editar(${c.id}, '${c.nome}', '${c.email}')" class="btn btn-warning btn-sm me-2">Editar</button>
                        <button onclick="deletar(${c.id})" class="btn btn-danger btn-sm">Excluir</button>
                    </td>
                `;

                lista.appendChild(tr);
            });
        });
}

// 💾 SALVAR
function salvar() {
    const nome = document.getElementById("nome").value;
    const email = document.getElementById("email").value;

    if (window.idEditando) {
        // 🔄 UPDATE
        fetch(`http://localhost:5107/api/clientes/${window.idEditando}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ nome, email })
        })
            .then(() => {
                alert("Cliente atualizado!");
                window.idEditando = null;
                buscar();
            });

    } else {
        // ➕ INSERT
        fetch("http://localhost:5107/api/clientes", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ nome, email })
        })
            .then(() => {
                alert("Cliente salvo!");
                buscar();
            });

        document.getElementById("nome").value = "";
        document.getElementById("email").value = "";
    }
}

// ❌ DELETAR
function deletar(id) {
    if (!confirm("Tem certeza que deseja excluir?")) return;

    fetch(`${api}/${id}`, {
        method: "DELETE"
    })
        .then(() => {
            alert("Cliente deletado!");
            buscar(); // atualiza lista
        });
}
// ✏️ EDITAR
function editar(id, nome, email) {
    console.log("clicou editar", id, nome, email);

    document.getElementById("nome").value = nome;
    document.getElementById("email").value = email;

    window.idEditando = id;
}
function voltarInicio() {
    // limpa busca
    document.getElementById("busca").value = "";

    // limpa lista (dados)
    document.getElementById("lista").innerHTML = "";

    // limpa form
    document.getElementById("nome").value = "";
    document.getElementById("email").value = "";

    // reseta edição
    window.idEditando = null;

    // 🔥 ESCONDE A TABELA
    document.getElementById("listaCard").style.display = "none";

    // 🔥 CENTRALIZA O FORM DE NOVO
    const container = document.getElementById("containerPrincipal");
    container.classList.remove("modo-lista");

}
function migrar() {
    window.location.href = "home.html";
}
function CadastroUsuario() {
    window.location.href = "CadastroUsuario.html";
}
async function salvarUsuario() {
    const nome = document.getElementById("nome").value;
    const email = document.getElementById("email").value;
    const nascimento = document.getElementById("DataNasc").value;
    const senha = document.getElementById("senha").value;
    const cpfRaw = document.getElementById("cpf").value;

    const cpf = tratarCPF(cpfRaw);

    const dados = {
        nome: nome,
        email: email,
        DataNasc: nascimento,
        senha: senha,
        cpf: cpf
    };
    console.log(dados);
    try {
        const resposta = await fetch(`http://localhost:5107/api/Usuario/`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(dados)
        });
        if (resposta.ok) {
            alert("Usuário Cadastrado com Sucesso!");
        } else {
            const erro = await resposta.text();
            alert("Erro:" + erro);
        }

    } catch (erro) {
        return BadRequest("ERRO:" + ex.ToString());
    }
    // } catch (err) {
    //      console.error(err);
    //      alert ("Erro na requisição");
    //  }

        window.location.href = "login.html";

}

document.querySelectorAll(".olho").forEach(function (icon) {
    icon.addEventListener("click", function () {
        const input = icon.previousElementSibling;

        if (input.type === "password") {
            input.type = "text";
            icon.classList.remove("bi-eye");
            icon.classList.add("bi-eye-slash");
        } else {
            input.type = "password";
            icon.classList.remove("bi-eye-slash");
            icon.classList.add("bi-eye");
        }
    });
});

function tratarCPF(cpf) {
    return cpf.replace(/\D/g, '');
}

function mascararCPF(input) {
    // pega só os números
    let numeros = input.value.replace(/\D/g, '');

    // limita a 11 números
    numeros = numeros.substring(0, 11);

    // aplica máscara padrão: 123.456.789-01
    numeros = numeros.replace(/(\d{3})(\d)/, '$1.$2');
    numeros = numeros.replace(/(\d{3})(\d)/, '$1.$2');
    numeros = numeros.replace(/(\d{3})(\d{1,2})$/, '$1-$2');

    // atualiza o input
    input.value = numeros;
}

const cpfInput = document.getElementById("cpf");
cpfInput.addEventListener ("input", () => mascararCPF(cpfInput));


const apiAuth = "http://localhost:5107/api/auth";

function login() {

   
    console.log("cpf:", document.getElementById("cpf"));
    console.log("senha:", document.getElementById("senha"));

    const cpf = document.getElementById("cpf").value.replace(/\D/g, "");
    const senha = document.getElementById("senha").value;
    const apiAuth = "http://localhost:5107/api/auth";

        fetch(`${apiAuth}/login`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ cpf, senha })
    })
    .then(res => {
        if (!res.ok) {
            throw new Error("CPF ou senha inválidos");
        }
        return res.json();
    })
    .then(data => {
        console.log("TOKEN:", data.token);

        // 🔥 SALVA TOKEN
        localStorage.setItem("token", data.token);

        alert("Login realizado!");

        // 🔄 REDIRECIONA
        window.location.href = "home.html";
    })
    .catch(err => {
        alert(err.message);
    });
}

function toggleMenu() {
    const menu = document.getElementById("menu");
    const overlay = document.getElementById("overlay");

    menu.classList.toggle("ativo");
    overlay.classList.toggle("ativo");
}

function fecharMenu() {
    document.getElementById("menu").classList.remove("ativo");
    document.getElementById("overlay").classList.remove("ativo");
}

function logout() {
    localStorage.removeItem("token");
    window.location.href = "login.html";
}