namespace Lectio.Api.Infrastructure.ExternalServices.GoogleBooks;

public record BookSeedCategoryConfig(string[] Subqueries, int MinTarget, int MaxTarget);

public static class BookSeedConfigs
{
    public static readonly Dictionary<string, BookSeedCategoryConfig> FiccionRomance = new()
    {
        ["Ficción"] = new(
            new[]
            {
                "novela", "ficción literaria", "cuentos", "relatos cortos",
                "ciencia ficción", "ciencia ficción espacial", "fantasía épica", "fantasía oscura",
                "aventuras", "novela de aventuras", "realismo mágico", "distopía",
                "ficción distópica juvenil", "terror", "cuentos de terror", "misterio",
                "novela negra", "thriller", "literatura latinoamericana", "literatura colombiana",
                "literatura española", "literatura contemporánea", "clásicos de la literatura",
                "novela histórica", "literatura juvenil", "saga fantástica", "ucronía",
                "ficción especulativa", "steampunk", "literatura fantástica"
            },
            500, 2000
        ),
        ["Romance"] = new(
            new[]
            {
                "novela romántica", "romance contemporáneo", "romance histórico", "romance de época",
                "romance victoriano", "amor", "amor prohibido", "amor de verano",
                "comedia romántica", "romance juvenil", "romance adolescente", "romance new adult",
                "romance paranormal", "romance fantástico", "suspenso romántico", "romance policíaco",
                "enemigos a amantes", "segunda oportunidad amor", "triángulo amoroso", "romance navideño",
                "romance en la playa", "romance millonario", "bodas novela", "chica busca chico"
            },
            500, 2000
        )
    };

    public static readonly Dictionary<string, BookSeedCategoryConfig> FantasiaTerror = new()
    {
        ["Fantasía"] = new(
            new[]
            {
                "fantasía épica", "fantasía oscura", "alta fantasía", "fantasía juvenil",
                "saga fantástica", "dragones novela", "magia y hechicería", "criaturas mágicas",
                "mundos fantásticos", "fantasía medieval", "elfos y enanos", "fantasía urbana",
                "fantasía histórica", "cuentos de hadas", "mitología fantástica", "literatura fantástica",
                "reinos y reyes fantasía", "fantasía heroica", "espada y brujería", "fantasía romántica",
                "portal fantástico", "fantasía steampunk", "fantasía gótica", "profecía y elegido",
                "academia de magia", "brujas novela", "vampiros fantasía", "hombres lobo novela",
                "ángeles y demonios", "fantasía apocalíptica"
            },
            500, 2000
        ),
        ["Terror"] = new(
            new[]
            {
                "terror", "terror psicológico", "terror sobrenatural", "casas embrujadas",
                "fantasmas novela", "posesión demoníaca", "terror gótico", "terror clásico",
                "relatos de terror", "cuentos de miedo", "horror cósmico", "criaturas del terror",
                "zombis novela", "vampiros terror", "hombres lobo terror", "brujería oscura",
                "terror paranormal", "terror juvenil", "terror victoriano", "monstruos novela",
                "terror slasher", "exorcismo novela", "terror rural", "terror urbano",
                "pesadillas novela", "terror lovecraftiano", "terror asiático", "terror sobrenatural juvenil",
                "terror apocalíptico", "leyendas de terror"
            },
            500, 2000
        )
    };

    public static readonly Dictionary<string, BookSeedCategoryConfig> MisterioAventura = new()
    {
        ["Misterio"] = new(
            new[]
            {
                "misterio", "novela de misterio", "novela negra", "thriller psicológico",
                "detectives novela", "crimen novela", "suspenso", "misterio policíaco",
                "asesinato misterio", "investigación criminal novela", "misterio clásico",
                "misterio juvenil", "espionaje novela", "misterio victoriano", "misterio nórdico",
                "misterio de época", "misterio urbano", "conspiración novela", "robo novela",
                "misterio sobrenatural"
            },
            500, 2000
        ),
        ["Aventura"] = new(
        new[]
        {
            "aventuras", "novela de aventuras", "exploración novela", "piratas novela",
            "supervivencia novela", "expedición novela", "viajes y aventuras",
            "aventura juvenil", "aventura histórica", "tesoros perdidos novela",
            "selva novela", "aventura de acción", "náufragos novela", "aventura marítima",
            "aventura en el desierto", "montañismo novela", "cazatesoros novela",
            "aventura épica", "aventura clásica", "aventura fantástica",
            "corsarios novela", "bucaneros novela", "aventura en alta mar",
            "islas perdidas novela", "mapa del tesoro novela", "safari novela",
            "aventura en la jungla", "amazonas novela", "expedición polar",
            "aventura ártica", "antártida novela", "aventura espacial",
            "aventureros novela", "cazadores de tesoros", "ruinas perdidas novela",
            "ciudad perdida novela", "templo perdido novela", "aventura arqueológica",
            "exploradores novela", "aventura de época", "aventura victoriana",
            "aventura del viejo oeste", "aventura en el río", "rally de aventura",
            "aventura extrema", "escape y evasión novela", "aventura de resistencia",
            "aventura juvenil clásica", "novela de capa y espada", "mosqueteros novela",
            "aventura de vikingos", "exploración submarina", "aventura submarina",
            "cueva perdida novela", "aventura en montaña", "escalada novela",
            "aventura de cazadores", "safari africano novela", "aventura colonial",
            "aventura de contrabandistas", "fuga y persecución novela"
        },
        500, 2000
    )
    };

    public static readonly Dictionary<string, BookSeedCategoryConfig> HistoriaCiencia = new()
    {
        ["Historia"] = new(
            new[]
            {
                "historia", "historia universal", "historia de Colombia", "historia latinoamericana",
                "historia antigua", "historia medieval", "historia moderna", "historia contemporánea",
                "segunda guerra mundial", "primera guerra mundial", "historia militar", "historia política",
                "revoluciones históricas", "historia del arte", "historia económica", "historia social",
                "civilizaciones antiguas", "imperio romano", "grecia antigua", "edad media",
                "colonización americana", "independencia latinoamericana", "guerra civil",
                "historia de la ciencia", "historia de la cultura"
            },
            500, 2000
        ),
        ["Ciencia"] = new(
            new[]
            {
                "ciencia", "divulgación científica", "física", "química", "biología",
                "astronomía", "matemáticas", "ciencia y tecnología", "neurociencia", "genética",
                "ecología", "medio ambiente", "cambio climático", "ciencia de datos",
                "inteligencia artificial", "robótica", "medicina", "psicología científica",
                "geología", "paleontología", "universo y cosmología", "energía y sostenibilidad",
                "ciencia para niños", "historia de la ciencia", "biotecnología"
            },
            500, 2000
        )
    };

    public static readonly Dictionary<string, BookSeedCategoryConfig> BiografiaPoesia = new()
    {
        ["Biografía"] = new(
            new[]
            {
                "biografía", "autobiografía", "memorias", "biografía histórica",
                "biografía de científicos", "biografía de artistas", "biografía de escritores",
                "biografía de líderes políticos", "biografía de músicos", "biografía de deportistas",
                "memorias de guerra", "memorias personales", "biografía empresarial",
                "biografía de mujeres destacadas", "biografía latinoamericana", "biografía de filósofos",
                "biografía de exploradores", "biografía de actores", "vidas ejemplares",
                "biografía no autorizada", "biografía novelada", "diarios personales",
                "cartas y correspondencia", "biografía de activistas", "biografía contemporánea"
            },
            500, 2000
        ),
        ["Poesía"] = new(
            new[]
            {
                "poesía", "poesía contemporánea", "poesía clásica", "poesía latinoamericana",
                "poesía española", "poesía colombiana", "poesía de amor", "poesía romántica",
                "poesía social", "poesía existencial", "poesía surrealista", "poesía modernista",
                "poesía del siglo XX", "poesía del siglo XIX", "antología poética", "poesía juvenil",
                "poesía mística", "poesía épica", "sonetos", "haiku",
                "poesía beat", "poesía feminista", "poesía experimental", "poesía y música",
                "poesía visual"
            },
            500, 2000
        )
    };

    public static readonly Dictionary<string, BookSeedCategoryConfig> Autoayuda = new()
    {
        ["Autoayuda"] = new(
            new[]
            {
                "autoayuda", "desarrollo personal", "crecimiento personal", "inteligencia emocional",
                "hábitos y productividad", "mindfulness", "meditación", "liderazgo personal",
                "motivación", "superación personal", "autoestima", "gestión del tiempo",
                "finanzas personales", "psicología positiva", "resiliencia", "manejo del estrés",
                "relaciones interpersonales", "comunicación asertiva", "propósito de vida",
                "coaching personal", "hábitos saludables", "mentalidad de crecimiento",
                "autoconocimiento", "espiritualidad y bienestar", "felicidad y bienestar"
            },
            500, 2000
        )
    };
}