namespace DesafioPokemon{
    /*
     * ESQUEMA DE CLASES
     * Menu: crea un objeto Pokedex, e instancia CombatSystem con los pokemon necesarios
     * Pokedex: almacena los objetos Pokemon que vayamos creando
     * PokemonAbstracto: una clase abstracta de la que derivan Pokemon y PokemonCPU. Funciona a modo de interfaz, para poder referenciara mbas clases de manera generica en el CombatSystem
     *      Pokemon: cada Pokemon creado por el jugador. Se almacenan en la Pokedex como objeto Pokemon.
     *      PokemonCPU: copia de Pokemon. La unica diferencia es que el ataque que realiza, se elige aleatoriamente.
     * Ataque: cada uno de los ataques. Existe una pool general de ataques, incluida en MetodosGenericos para optimizar memoria. Cada Pokemon aprende 4 movimientos. los ataques pueden ser fisicos o especiales (AtaqueFisico, AtaqueEspecial)
     * Tipo: enumeracion que contiene los distintos tipos elementales, tanto de los pokemon, como de sus movimientos
     * CombatSystem: se encarga de manejar dos COPIAS de pokemon de la pokedex, enfrentandolas en combate
     * MetodosGenericos: es una clase que contiene metodos genericos utilizados en varias clases del programa, para optimizar memoria sin la necesidad de redefinir estos metodos en cada clase
     */
    internal class Program{
        static void Main(string[] args){    
            //Se crea un objeto Menu y se llama a MostrarMenu()
            //Menu tiene su propio objeto Pokedex y crea un CombatSytem volatil a la hora de iniciar un combate.
            Menu menu = new Menu();
            menu.MostrarMenu();
        }
    }
    public class Menu
    {
        Random rnd = new Random();
        Pokedex pokedex = new Pokedex();
        //indicePokemonJugador e indicePokemonCPU incluyen el indice correspondiente a la pokedex de los pokemons seleccionados para el combate
        int indicePokemonJugador = 0;
        int indicePokemonCPU = 0;
        public Menu() { }
        public void MostrarMenu()
        {
            //Muestra por consola el menu principal
            Console.ForegroundColor = ConsoleColor.White;
            do
            {
                Console.WriteLine(
                    "A. CREAR POKEMON \r\n" +
                    "B. CONSULTAR POKEDEX \r\n" +
                    "C. INICIAR COMBATE");
            } while (ElegirOpcion() == true);
        }
        bool ElegirOpcion()
        {
            //De acuerdo a la tecla que pulsa el usuario, se le redirige a otras secciones del programa, de acuerdo al texto mostrada en MostrarMenu();
            //Es una funcion Bool ya que se usa como condicion de un bucle do while. Si el usuario pulsa la tecla Escape, el programa terminará
            Console.Write("Elige una de las opciones o pulsa ESC para cerrar la aplicacion: ");
            ConsoleKey key = Console.ReadKey(true).Key;
            Console.Clear();
            if (key == ConsoleKey.A)
            {
                CreacionPokemon();
            }
            if (key == ConsoleKey.B)
            {
                ConsultarPokedex();
            }
            if (key == ConsoleKey.C)
            {
                Combate();
            }
            if (key == ConsoleKey.Escape)
            {
                return false;
            }
            return true;

        }
        void CreacionPokemon()
        {

            Console.WriteLine("Has elegido la opcion Crear Pokemon");
            pokedex.AnadirPokemon();
        }
        void ConsultarPokedex()
        {
            Console.WriteLine("Has elegido la opcion Consultar Pokedex");
            //indicePokemonJugador = rnd.Next(pokedex.Pokemons.Length);
            //indicePokemonCPU = rnd.Next(pokedex.Pokemons.Length);
            pokedex.Consultar(ref indicePokemonJugador, ref indicePokemonCPU);
        }
        void Combate()
        {
            Console.WriteLine("Has elegido la opcion Combate");
            if (pokedex.ComprobarPokedex("Para iniciar un combate, primero debes crear un pokemon")) return;
            Console.WriteLine($"Los pokemon que participarán en el combate son: {pokedex.Pokemons[indicePokemonJugador].Nombre} y {pokedex.Pokemons[indicePokemonCPU].Nombre}");
            //Si el usuario quiere modificar la eleccion de los pokemon que participan en el combate, se vuelve a mostrar la pokedex, para que los elija. Por defecto, estos seran el primer pokemon de la pokedex
            if (MetodosGenericos.ConfirmarSeleccion("¿Quieres elegir otros Pokemon que participen en el combate? (S/N)")) pokedex.Consultar(ref indicePokemonJugador, ref indicePokemonCPU);
            CombatSystem Combate = new CombatSystem(pokedex.DevolverPokemon(indicePokemonJugador), pokedex.DevolverPokemon(indicePokemonCPU));
            Combate.IniciarCombate();
            Console.Clear();
        }
    }
    public class Pokedex{
        //pokemons es un array de objetos Pokemon
        Pokemon[] pokemons;
        public Pokedex() {
            //al inicializar la pokedex, el array de Pokemons tiene unicamente un indice, luego este indice se amplia de manera dinamica al ir añadiendo pokemons
            pokemons = new Pokemon[0];
        }
        public Pokemon[] Pokemons { get { return pokemons; } set { pokemons = value; } }
        public Pokemon DevolverPokemon(int i) => pokemons[i];
        public void Consultar(ref int pokemonJugador, ref int pokemonCPU) {
            //Itera la pokedex para mostrar las estadisticas de cada Pokemon, asi como elegir los pokemon que participaran en el siguiente combate.
            //debe comprobar el valor de la pokedex para poder funcionar sin provocar excepciones
            if (ComprobarPokedex("Para consultar la Pokedex, primero debes añadir pokemons!")) return;
            ConsoleKey tecla;
            int indiceConsulta = 0;
            do {
                pokemons[indiceConsulta].MostrarEstadisticas();
                Console.WriteLine("Pulsa Z para elegir este pokemon como aliado");
                Console.WriteLine("Pulsa X para elegir este pokemon como adversario");
                tecla = Console.ReadKey(true).Key;
                if (tecla == ConsoleKey.Z) pokemonJugador = indiceConsulta;
                if (tecla == ConsoleKey.X) pokemonCPU = indiceConsulta;
                if (tecla == ConsoleKey.LeftArrow) indiceConsulta--;
                if (tecla == ConsoleKey.RightArrow) indiceConsulta++;
                if (indiceConsulta < 0) indiceConsulta = 0;
                if (indiceConsulta > pokemons.Length-1) indiceConsulta = pokemons.Length - 1;
                Console.Clear();
            } while (tecla!=ConsoleKey.Escape);
        }
        public bool ComprobarPokedex(string mensaje) {
            //Comprueba que la pokedex tenga informacion almacenada, esto funciona ya que el tamaño de la pokedex se actualiza con cada Pokemon que se añade en ella
            if (pokemons.Length == 0){
                Console.WriteLine(mensaje);
                Console.ReadKey(true);
                Console.Clear();
                return true;
            }
            return false;
        }
        public void AnadirPokemon(){
            //a traves de este metodo, se crea un objeto Pokemon y se añade a la pokedex (o no, a lo mejor el usuario se arrepiente)
            int vida = MetodosGenericos.IntroducirNumero(25, "Introduce la vida del Pokemon: ");
            int ataqueFisico = MetodosGenericos.IntroducirNumero(1, "Introduce el ataque del Pokemon: ");
            int ataqueEspecial = MetodosGenericos.IntroducirNumero(1, "Introduce el ataque especial del Pokemon: ");
            int defensaFisica = MetodosGenericos.IntroducirNumero(1, "Introduce la defensa del Pokemon: ");
            int defensaEspecial = MetodosGenericos.IntroducirNumero(1, "Introduce la defensa especial del Pokemon: ");
            int velocidad = MetodosGenericos.IntroducirNumero(10, "Introduce la velocidad del Pokemon: ");
            Console.Write("Introduce el tipo del Pokemon: ");
            //Se asigna el tipo del pokemon
            Tipo tipo = AsignarTipo(Console.ReadLine());
            Pokemon pokemonAAnadir = new Pokemon(vida, ataqueFisico, defensaFisica, ataqueEspecial, defensaEspecial, velocidad, tipo);
            pokemonAAnadir.MostrarEstadisticas();
            //Si el usuario quiere modificar alguna de las estadisticas, se le da la opcion de hacerlo.
            if (MetodosGenericos.ConfirmarSeleccion("¿Quieres modificar alguna de las estadisticas (S/N)?")) pokemonAAnadir.ModificarEstadisticas();
            //Si el usuario quiere crear el pokemon, se le asigna un nombre, se eligen sus movimientos y se añade a la `pokedex
            if(MetodosGenericos.ConfirmarSeleccion("¿Estas seguro de que quieres crear este pokemon (S/N)?")){
                pokemonAAnadir.AsignarNombre();
                pokemonAAnadir.ElegirMovimientos();
                ModificarArray(pokemonAAnadir);
            }
        }
        Tipo AsignarTipo(string _tipo){
            //Sirve para asignar un tipo durante la creacion de Pokemon. Complementa a AnadirPokemon()
            //Se pasa _tipo a UpperCase para evitar cualquier posible disonancia con el string al que se compara. A ojos del programa, Lucha = LUCHa = luCHa = lucha = LUCHA
            _tipo = _tipo.ToUpper();
            for (int i = 0; i < MetodosGenericos.traducirTipoAString.Length; i++){
                //se recorren todos los valores de traducirTipoAString. Si el tipo string introducido coincide con uno de los valores, el pokemon pasara a tener dicho tipo.
                if (MetodosGenericos.traducirTipoAString[i] == _tipo){
                    return (Tipo)i;
                }
            }
            //si ninguno de los valores coincide, el pokemon sera de tipo Normal
            return Tipo.Normal;
        }
        void ModificarArray(Pokemon pokemonAAnadir){
            //esta funcion añade el Pokemon creado en AñadirPokemon() al array pokemons. Complementa AnadirPokemon()
            //para ello, crea un array que desaparece tras ejecutarse la funcion
            Pokemon[] arrayVolatil = this.pokemons;
            //el array original, pokemons, se "sobreescribe", su tamaño aumenta en un indice y pierde todo su contenido
            this.pokemons = new Pokemon[arrayVolatil.Length + 1];
            //pokemons recupera su contenido original
            for (int i = 0; i < arrayVolatil.Length; i++)
            {
                this.pokemons[i] = arrayVolatil[i];
            }
            //se añade el pokemon creado en el ultimo indice de pokemons
            this.pokemons[pokemons.Length-1]= pokemonAAnadir;
        }

    }
    public abstract class PokemonAbstracto {
        //estadisticas y elementos comunes de TODOS los pokemon
        string? nombre;
        protected int vidaMax;
        protected int vida;
        protected int ataqueFisico;
        protected int defensaFisica;
        protected int ataqueEspecial;
        protected int defensaEspecial;
        protected int velocidad;
        protected Tipo tipo;
        protected Ataque[] movimientos = new Ataque[4];
        //propiedades
        public int VidaMax { get { return vidaMax; } set { vidaMax = value; } }
        public Ataque[] Movimientos { get { return movimientos; }}
        public int Vida { get { return vida; } set { vida = value; } }
        public int AtaqueFisico { get { return ataqueFisico; } }
        public int DefensaFisica { get { return defensaFisica; } }
        public int AtaqueEspecial { get { return ataqueEspecial; } }
        public int DefensaEspecial { get { return defensaEspecial; } }
        public int Velocidad { get { return velocidad; } set { velocidad = value; } }
        public string Nombre { get { return nombre;} set { nombre = value;} }
        //Tipo es una enumeracion, por lo que al trabajar con éste, será encesario definir previamente algun tipo de traducción (Tipo.Fuego = "Fuego")
        public string TipoString { get { return MetodosGenericos.traducirTipoAString[(int)tipo]; } }
        public Tipo Tipo { get { return tipo; } }
        public PokemonAbstracto() {
            //Este constructor esta hecho para cumplir con el requisito de que los Pokemon tengan estadisticas aleatorizadas.
            Random rnd = new Random();
            vidaMax = rnd.Next(25,450);
            vida = vidaMax;
            ataqueFisico = rnd.Next(1,500);
            defensaFisica = rnd.Next(1, 500);
            ataqueEspecial = rnd.Next(1, 500);
            defensaEspecial = rnd.Next(1, 500);
            velocidad = rnd.Next(10, 500);
            tipo = (Tipo)rnd.Next(8);
        }
        public PokemonAbstracto(int _vidaMax, int _ataqueFisico, int _defensaFisica, int _ataqueEspecial, int _defensaEspecial, int _velocidad, Tipo _tipo) {
            vidaMax = _vidaMax;
            vida = vidaMax;
            ataqueFisico = _ataqueFisico;
            defensaFisica = _defensaFisica;
            ataqueEspecial = _ataqueEspecial;
            defensaEspecial = _ataqueEspecial;
            velocidad = _velocidad;
            tipo = _tipo;
            /* OBSERVACION
             * El siguiente comentario es de cuando PokemonAbstracto no existia, y todos los pokemon eran simplemente de la clase Pokemon.
            en la pokedex, los pokemon se almacenan como objetos de clase Pokemon, y luego se convierten a PokemonAbstracto al pasar a CombatSystem. Aqui el tipo ya esta definido, por lo que no es un problema,
            y se incluye como parametro del constructor.
            esto me generaba dudas. El tipo del pokemon no puedo realmente asignarlo de manera directa, ya que se trata de una enumeracion.
            Debo traducir la informacion introducida por el usuario a la enumeracion Tipo. Para ello, hago uso de una funcion definida en Pokemon, que podre llamar desde Pokemon,
            y tambien podre usar cuando tenga que mostrar el tipo Elemental del ataque. No quiero copiar esta funcion dentro de la clase Pokedex, por lo que llamare al metodo AsignarTipo desde el constructor*/
        }
        public double CalcularDebilidad(Tipo tipoAtaque){
            //Esta funcion devuelve el multiplicador de daño del ataque, de acuerdo al tipo Elemental del ataque y del pokemon que sufre dicho ataque
            //No se tiene en cuenta el tipo del Pokemon que efectua el ataque.
            //Solo se tienen en cuenta los casos MUY EFECTIVOS, POCO EFECTIVOS y NO EFECTIVOS
            if (tipoAtaque == Tipo.Fuego && (tipo == Tipo.Planta)) return 2;
            if (tipoAtaque == Tipo.Planta && (tipo == Tipo.Fuego)) return 0.5;
            if (tipoAtaque == Tipo.Planta && (tipo == Tipo.Agua)) return 2;
            if (tipoAtaque == Tipo.Agua && (tipo == Tipo.Planta)) return 0.5;
            if (tipoAtaque == Tipo.Agua && (tipo == Tipo.Fuego)) return 2;
            if (tipoAtaque == Tipo.Fuego && (tipo == Tipo.Agua)) return 0.5;
            if (tipoAtaque == Tipo.Electrico && tipo == Tipo.Agua) return 2;
            if (tipoAtaque == Tipo.Agua && (tipo == Tipo.Electrico)) return 0.5;
            //Los ataques de tipo NORMAL no hacen NADA de daño a los pokemon tipo Normal
            if (tipoAtaque == Tipo.Fantasma && (tipo == Tipo.Normal)) return 0;
            if (tipoAtaque == Tipo.Fantasma && (tipo == Tipo.Psiquico || tipo == Tipo.Fantasma)) return 2;
            //Importante en este caso no es necesario incluir los ataques Fantasma a Fantasma, ya que estan incluidos en el anterior.
            if (tipoAtaque == Tipo.Psiquico && (tipo == Tipo.Fantasma)) return 0.5;
            if (tipoAtaque == Tipo.Lucha && (tipo == Tipo.Normal || tipo == Tipo.Siniestro)) return 2;
            if ((tipoAtaque == Tipo.Normal || tipoAtaque == Tipo.Siniestro) && (tipo == Tipo.Lucha)) return 0.5;
            if (tipoAtaque == Tipo.Siniestro && (tipo == Tipo.Psiquico)) return 2;
            if (tipoAtaque == Tipo.Psiquico && (tipo == Tipo.Siniestro)) return 0.5;
            if (tipoAtaque == Tipo.Psiquico && (tipo == Tipo.Lucha)) return 2;
            if (tipoAtaque == Tipo.Lucha && (tipo == Tipo.Psiquico)) return 0.5;
            return 1;
        }
        public void ElegirMovimientos()
        {
            //Este metodo sirve para elegir los movimientos que aprende el pokemon, durante su creacion
            Random rnd = new Random();
            for (int indiceMovimiento = 0; indiceMovimiento < movimientos.Length; indiceMovimiento++)
            {
                for (int i = 0; i < MetodosGenericos.poolMovimientos.Length; i++)
                {
                    //Se muestran todos los movimientos junto a las teclas vinculadas a su seleccion
                    Console.WriteLine($"{MetodosGenericos.teclasMovimientos[i]} | {MetodosGenericos.poolMovimientos[i].TextoAtaque} | {MetodosGenericos.poolMovimientos[i].Potencia} | {MetodosGenericos.poolMovimientos[i].Precision} | {MetodosGenericos.traducirTipoAString[(int)MetodosGenericos.poolMovimientos[i].Tipo]}"); //FORMATEAR STRING
                }
                /*OBSERVACION
                aqui me surgio un dilema, usar strings o teclas para elegir el movimiento a aprender.
                usar Strings: posible NULL, debe coincidir con el nombre //movimiento aleatorio si el string es NULL o no coincide con ningun ataque
                usar Teclas: necesaria otra matriz u enumeracion, //movimiento aleatorio si la tecla no esta registrada //mucho mejor usar Teclas*/
                Console.Write("Pulsa la tecla correspondiente al movimiento que quieres que aprenda tu pokemon: ");
                ConsoleKey tecla = Console.ReadKey(true).Key;
                for (int i = 0; i < MetodosGenericos.poolMovimientos.Length; i++)
                {
                    if (tecla == MetodosGenericos.teclasMovimientos[i])
                    {
                        movimientos[indiceMovimiento] = MetodosGenericos.poolMovimientos[i];
                        break;
                    }
                    //Si la tecla introducida por el usuario no coincide con ninguna de las teclas disponibles, se elige un movimiento aleatorio
                    if (i == MetodosGenericos.poolMovimientos.Length - 1)
                    {
                        movimientos[indiceMovimiento] = MetodosGenericos.poolMovimientos[rnd.Next(0, MetodosGenericos.poolMovimientos.Length - 1)];
                    }
                }
                //Si el ataque que ha elegido el usuario ya ha sido aprendido, se cambia por otro ataque aleatorio. Esto se repite hasta que los ataques no coincidan.
                for (int comprobarMovimiento = 0; comprobarMovimiento < indiceMovimiento; comprobarMovimiento++)
                {
                    while (movimientos[comprobarMovimiento] == movimientos[indiceMovimiento])
                    {
                        movimientos[indiceMovimiento] = MetodosGenericos.poolMovimientos[rnd.Next(0, MetodosGenericos.poolMovimientos.Length - 1)];
                    }
                    //Se recorren los movimientos que ya han sido elegidos para que el jugador no pueda elegir movimientos ya aprendidos
                }
                Console.Clear();
            }
        }
        protected ConsoleKey[] teclasAtaques = { ConsoleKey.Q, ConsoleKey.W, ConsoleKey.E, ConsoleKey.R };
        public void MostrarMovimientos()
        {
            //Se muestran todos los movimientos aprendidos por el pokemon, junto a la tecla correspondientes a cada uno de ellos.
            for (int i = 0; i < movimientos.Length; i++)
            {
                Console.ForegroundColor = MetodosGenericos.coloresTipo(movimientos[i].Tipo);
                Console.WriteLine($"{teclasAtaques[i].ToString()} | {movimientos[i].TextoAtaque} | {movimientos[i].Potencia} |{movimientos[i].Precision} | {MetodosGenericos.traducirTipoAString[(int)movimientos[i].Tipo]}");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
        public void MostrarEstadisticas(){
            //Se muestran las estadisticas del pokemon
            if (nombre != null) Console.WriteLine(nombre.ToUpper());
            Console.WriteLine(
                $"Salud: {vida}/{vidaMax} \r\n" +
                $"Ataque Fisico: {ataqueFisico} \r\n" +
                $"Ataque Especial: {ataqueEspecial} \r\n" +
                $"Defensa Fisica: {defensaFisica} \r\n" +
                $"Defensa Especial: {defensaEspecial} \r\n" +
                $"Velocidad: {velocidad} \r\n" +
                $"Tipo: {tipo} \r\n");
        }
        protected bool ElegirOpcion(){
            //Este metodo SOLO se llama al modificar el usuario una de las estadisticas del pokemon. Se le da la opcion de modificar una de las estadisticas o volver hacia atras.
            Console.Write("Elige una de las opciones o pulsa ESC para volver hacia atrás. ");
            ConsoleKey key = Console.ReadKey(true).Key;
            Console.Clear();
            if (key == ConsoleKey.A) {
                vida = MetodosGenericos.IntroducirNumero(vida, 25, "Introduce la nueva vida del Pokemon: ");
                vidaMax = vida;
            }
            if (key == ConsoleKey.B) ataqueFisico = MetodosGenericos.IntroducirNumero(ataqueFisico, 1, "Introduce el nuevo ataque del Pokemon: ");
            if (key == ConsoleKey.C) ataqueEspecial = MetodosGenericos.IntroducirNumero(ataqueEspecial, 1, "Introduce el nuevo ataque especial del Pokemon: ");
            if (key == ConsoleKey.D) defensaFisica = MetodosGenericos.IntroducirNumero(defensaFisica, 1, "Introduce la nueva defensa del Pokemon: ");
            if (key == ConsoleKey.E) defensaEspecial = MetodosGenericos.IntroducirNumero(defensaEspecial, 1, "Introduce la nueva defensa especial del Pokemon: ");
            if (key == ConsoleKey.F) velocidad = MetodosGenericos.IntroducirNumero(velocidad, 10, "Introduce la nueva velocidad del Pokemon: ");
            if (key == ConsoleKey.Escape) return false;
            return true;

        }    
        public virtual Ataque ElegirMovimiento(ref PokemonAbstracto atacante, ref PokemonAbstracto defensor){
            //es un metodo virtual ya que se sobreescribe exclusivamente en los objetos PokemonCPU.
            //permite al jugador elegir el movimiento que realizara su pokemon. En el caso de PokemonCPU, el jugador no puede hacer esto, por lo que se sobreescribe
            while (true){
                Console.WriteLine("Pulsa la tecla correspondiente al ataque que quieres usar.");
                for (int i = 0; i < movimientos.Length; i++){
                    //Se muestran todos los movimientos del Pokemon, pero sin mostrar la potencia ni la precision, ni el tipo. (Por eso no uso MostrarMovimientos())
                    Console.ForegroundColor = MetodosGenericos.coloresTipo(movimientos[i].Tipo);
                    Console.WriteLine($"{teclasAtaques[i].ToString()} | {movimientos[i].TextoAtaque}");
                }
                Console.ForegroundColor = ConsoleColor.White;
                ConsoleKey tecla = Console.ReadKey(true).Key;
                for (int i = 0; i < teclasAtaques.Length; i++){
                    //Se recorre teclasAtaques para ver si alguna de ellas coincide con la tecla pulsada por el usuario
                    if (tecla == teclasAtaques[i]){
                        //En ese caso, el movimiento corresponde al indice actual de teclasAtaques
                        movimientos[i].Atacante = atacante;
                        movimientos[i].Defensor = defensor;
                        return movimientos[i];
                    }
                }
                //Si el usuario pulsa una tecla incorrecta, sale un mensaje por pantalla que le informa de ello
                Console.WriteLine("Por favor, pulsa una tecla correcta");
            }
        }   
    }
    public class Pokemon:PokemonAbstracto{
        public Pokemon():base() { }
        public Pokemon(int _vidaMax, int _ataqueFisico, int _defensaFisica, int _ataqueEspecial, int _defensaEspecial, int _velocidad, Tipo _tipo):base(_vidaMax, _ataqueFisico,_defensaFisica,_ataqueEspecial,_defensaEspecial,_velocidad,_tipo){}
        public Pokemon(Pokemon pokemonCopia) : base(pokemonCopia.VidaMax, pokemonCopia.AtaqueFisico, pokemonCopia.defensaFisica, pokemonCopia.AtaqueEspecial, pokemonCopia.DefensaEspecial, pokemonCopia.Velocidad, pokemonCopia.Tipo) {
            base.movimientos = pokemonCopia.Movimientos;
            base.Nombre = pokemonCopia.Nombre;
        }
        public void AsignarNombre(){
            //Funcion usada para ponerle un nombre a un pokemon.
            do{
                Console.Write("Por ultimo, ponle un nombre a este pokemon: ");
                base.Nombre = Console.ReadLine();
                if (base.Nombre == null) base.Nombre = "PLACEHOLDER";
            } while (MetodosGenericos.ConfirmarSeleccion("¿Estas seguro de que quieres ponerle este nombre a tu pokemon (S/N)?") == false);
        }
        public void ModificarEstadisticas()
        {
            //menu para la modificacion de estadisticas. Solo se hace con objetos Pokemon durante la creacion de estos, por lo que no es un problema que esté aquí.
            Pokemon pokemonTemporal = this;
            do
            {
                Console.WriteLine("\r\n" +
                    "A. SALUD \r\n" +
                    "B. ATAQUE FISICO \r\n" +
                    "C. ATAQUE ESPECIAL \r\n" +
                    "D. DEFENSA FISICA \r\n" +
                    "E. DEFENSA ESPECIAL \r\n" +
                    "F. VELOCIDAD");
            } while (base.ElegirOpcion() == true);
        }

    }
    public class PokemonCPU : Pokemon {
        public PokemonCPU(Pokemon pokemon):base(pokemon.Vida, pokemon.AtaqueFisico,pokemon.DefensaFisica,pokemon.AtaqueEspecial,pokemon.DefensaEspecial,pokemon.Velocidad,pokemon.Tipo){
            base.movimientos = pokemon.Movimientos;
            base.Nombre = pokemon.Nombre;
        }
        public override Ataque ElegirMovimiento(ref PokemonAbstracto atacante, ref PokemonAbstracto defensor){
            //el usuario no deberia elegir el movimiento que usa el pokemon al que se enfrenta. Es por esto que los PokemonCPU hacen uso de una "inteligencia artificial" bastante arcaica: determinan la debilidad y buscan un movimiento de ese mismo tipo
            Random rnd = new Random();
            Ataque movimiento = movimientos[rnd.Next(movimientos.Length)];
            //Se calcula la debilidad del pokemon objetivo
            Tipo debilidad = CalcularDebilidad(defensor);
            for (int i = 0; i < movimientos.Length; i++) {
                if (movimientos[i].Tipo == debilidad) movimiento = movimientos[i];
            }
            movimiento.Atacante = atacante;
            movimiento.Defensor = defensor;
            return movimiento;
        }
        //Este metodo devuelve un valor Tipo, que corresponde a la debilidad del pokemon que recibe el ataque
        Tipo CalcularDebilidad(PokemonAbstracto pokemonObjetivo) => MetodosGenericos.debilidades[(int)pokemonObjetivo.Tipo];
    }
    public abstract class Ataque{
        //para que el programa sea mas simple, solo habra un tipo por Pokemon.
        protected Tipo tipo;
        protected int potencia;
        protected int precision;
        protected Random rnd = new Random();
        protected string? textoAtaque;
        //las propiedades atacante y defensor no se inicializan, se modifican en cada turno
        //en el caso de defensor, esto es bueno, ya que siempre se elige el pokemon objetivo (esto es mas relevante en combates 2c2, aunque no existen en este programa)
        protected PokemonAbstracto? atacante;
        protected PokemonAbstracto? defensor;
        public string TextoAtaque { get { return textoAtaque; } }
        public int Potencia { get { return potencia; } }
        public int Precision { get { return precision; } }
        public Tipo Tipo { get { return tipo; } }
        //El pokemon atacante y defensor podrian haberse incluido como parametros a modificar dentro de la clase Ataque, pero me parece mas efectivo usarlos de esta manera ya que asi
        //se modifican SOLO durante el combate y no se modifica el contenido de los objetos tipo Ataque. Ademas, en combates 2c2 de Pokemon, esto es bastante mas conveniente.
        public PokemonAbstracto Atacante { get { return atacante; } set { atacante = value; } }
        public PokemonAbstracto Defensor { get { return defensor; } set { defensor = value; } }
        public Ataque(string _textoAtaque = "Ataque sin Nombre", int _potencia = 100, int _precision = 100, Tipo _tipo = Tipo.Normal){
            //Constructor del objeto Ataque. Todo ataque tiene un nombre, potencia, precision y tipo
            textoAtaque = _textoAtaque;
            potencia = _potencia;
            precision = _precision;
            tipo = _tipo;
        }
        //El calculo del daño inflingido en el Ataque es distinto, segun el ataque sea especial o fisico.
        //Tambien pretendia implementar ataques mas complicados como Ataque Rapido o Desenrollar, pero al final no lo he hecho. Es un poco mas complicado de lo que creia ya que "rompen con toda la logica establecida" en el CombatSystem
        public void UsarAtaque(PokemonAbstracto defensor) {
            defensor.Vida=(int)(CalcularDaño(defensor));
            MensajeAtaque();
        }
        //ProbabilidadAtaque determina si el movimiento de logra impactar en el pokemon objetivo de manera precisa, en base a la precision del ataque
        protected bool ProbabilidadAtaque() => rnd.Next(100)<=precision; 
        //Devuelve el daño del ataque a realizar. El calculo cambia, segun el ataque sea fisico o especial
        public abstract int CalcularDaño(PokemonAbstracto defensor);
        void MensajeAtaque(){
            //A la hora de usar los ataques, se muestra un mensaje por consola que mostrara {pokemon.Nombre} ha usado {ataque.nombre}
            //esto tambien hubiera podido hacerlo con una variable "ataqueElegido" dentro de los objetos Pokemon
            Console.WriteLine($"{atacante.Nombre} ha usado {textoAtaque}");
        }
    }
    public class AtaqueFisico : Ataque{
        //Los ataques pueden dividirse en ataques fisicos y magicos. Las estadisticas que tendran en cuenta de los pokemon son distintas, pero funcionan de manera similar
        public AtaqueFisico(string _textoAtaque, int _potencia, int _precision, Tipo _tipo) : base(_textoAtaque, _potencia, _precision, _tipo) { }
        public override int CalcularDaño(PokemonAbstracto defensor){
            //calculo del daño de ataque fisico. Si el ataque falla, hace 0 de daño. Se llama a probabilidadAtaque para que la herencia tenga mas sentido. No estaba muy seguro de que cumpliera, de otra manera, con los 2 metodos minimos por clase.
            if (ProbabilidadAtaque()) return (int)(defensor.Vida - (atacante.AtaqueFisico * potencia / defensor.DefensaFisica) * defensor.CalcularDebilidad(tipo));
            return 0;
        }
    }
    public class AtaqueEspecial : Ataque{
        //Los ataques pueden dividirse en ataques fisicos y magicos. Las estadisticas que tendran en cuenta de los pokemon son distintas, pero funcionan de manera similar
        public AtaqueEspecial(string _textoAtaque, int _potencia, int _precision, Tipo _tipo) : base(_textoAtaque, _potencia, _precision, _tipo) { }
        public override int CalcularDaño(PokemonAbstracto defensor){
            //calculo del daño de ataque especial. Si el ataque falla, hace 0 de daño
            if (ProbabilidadAtaque()) return (int)(defensor.Vida - (atacante.AtaqueEspecial * potencia / defensor.DefensaEspecial) * defensor.CalcularDebilidad(tipo));
            return 0;
        }
    }
    //enumeracion que reune todos los tipos elementales del programa. Se hace uso de matrices complementarias para convertir sus valores a tipo string
    public enum Tipo { Fuego, Planta, Agua, Electrico, Fantasma, Normal, Lucha, Siniestro, Psiquico }
    public class CombatSystem {
        PokemonAbstracto pokemonJugador;
        PokemonAbstracto pokemonCPU;
        public CombatSystem(Pokemon _pokemonJugador){
            //Este constructor está hecho para cumplir el requisito de que el Pokemon enemigo tenga sus estadisticas aleatorizadas
            pokemonJugador = new Pokemon(_pokemonJugador);
            pokemonCPU = new PokemonCPU(new Pokemon());
        }
        public CombatSystem(Pokemon _pokemonJugador, Pokemon _pokemonCPU){
            pokemonJugador= new Pokemon(_pokemonJugador);
            pokemonCPU= new PokemonCPU(_pokemonCPU);
            /*OBSERVACION
             * No se por qué, la pokedex se actualiza con los combates reduciendo la vida de los Pokemon, no tiene sentido ya que lo que hace CombatSystem es usar copias de los objetos de la pokedex
             * Por algun motivo, se interpretaban como parametros pasados por referencia.
             * Usando la palabra clave new, se especifica que son objetos separados, completamente nuevos, y ya no se actualiza el valor de la pokedex
             * Para esto, fue necesario aplicar polimorfismo en el constructor de los objetos de la clase Pokemon. Ahora mismo no se si hay otra manera de hacerlo */
        }
        public void IniciarCombate(){
            //Se inicia el combate
            bool condicion = true;
            //Mientras ningun pokemon salga victorioso, el combate continua
            while (condicion) {
                MostrarSalud();
                //Se eligen los movimientos de ambos pokemon, y se recoge en un array de tipo Ataque
                Ataque[] movimientos = OrdenAtaque(pokemonJugador.ElegirMovimiento(ref pokemonJugador, ref pokemonCPU), pokemonCPU.ElegirMovimiento(ref pokemonCPU, ref pokemonJugador));
                foreach (Ataque ataque in movimientos ) {
                    ataque.UsarAtaque(ataque.Defensor);
                    Console.ReadKey(true);
                    //Se comprueba que ningun pokemon haya ganado
                    if (ComprobarVictoria()){
                        //En caso de que un pokemon gane, condicion pasa a ser false, y el bucle while se detendra
                        condicion = false;
                        break;
                    }
                }
                Console.Write("Pulsa cualquier tecla para continuar!");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
        Ataque[] OrdenAtaque(Ataque ataqueJugador, Ataque ataqueCPU) {
            //Se devuelve un array de Ataques, de acuerdo a la velocidad de los pokemon.
            if (pokemonJugador.Velocidad >= pokemonCPU.Velocidad) return new Ataque[] {ataqueJugador, ataqueCPU};
            return new Ataque[] { ataqueCPU, ataqueJugador };
            /*OBSERVACION
            A traves de las propiedades Atacante y Defensor de la clase Ataque, es posible referenciar a ambos objetos y actualizarlos en el combate a traves de un bucle for.
            Anteriormente, esto no era posible por como habia acabado planteando el programa (tenian que ser muchas estructuras if con comprobaciones tras cada ataque). Asi es mucho mas optimo
            Eso si, lo que no me cuadra es que se este actualizando la vida, ya que la vida no se pasa por referencia. Esto ya habia pasado antes con el constructor de CombatSystem, 
            se actualizaba la pokedex tras cada combate y los pokemon perdian vida de forma permanente.*/
        }
        bool ComprobarVictoria() {
            //Se comprueba si hay un ganador del combate 
            if (pokemonJugador.Vida >0 && pokemonCPU.Vida >0) return false;
            //Si hay un ganador del combate, se muestra por pantalla.
            if (pokemonJugador.Vida <= 0) Console.WriteLine($"El ganador del combate es {pokemonCPU.Nombre}");
            if (pokemonCPU.Vida <= 0) Console.WriteLine($"El ganador del combate es {pokemonJugador.Nombre}");
            return true;
        }
        void MostrarSalud(){
            //Se muestra la salud de los Pokemon por pantalla durante el combate.
            Console.WriteLine($"{pokemonCPU.Nombre,40}:{pokemonCPU.Vida}/{pokemonCPU.VidaMax}");
            Console.WriteLine($"{pokemonJugador.Nombre}:{pokemonJugador.Vida}/{pokemonJugador.VidaMax}");
        }
    }
    public class MetodosGenericos() {
        static public int IntroducirNumero(int valorPorDefecto, string textoAMostrar) {
            //Muestra texto por pantalla y le pide al usuario que introduzca un valor, para luego asignarlo a una variable.
            //Hace las conversiones necesarias para que la variable no tenga un valor nulo, evitando asi excepciones
            Console.Write(textoAMostrar);
            int n = valorPorDefecto;
            int.TryParse(Console.ReadLine(), out n);
            if (n > valorPorDefecto) return n;
            return valorPorDefecto;
        }
        static public int IntroducirNumero(int valorAnterior, int valorMinimo, string textoAMostrar) {
            //Se aplica polimorfismo a la funcion anterior para modificar valores ya establecidos. Esto se utiliza al modificar estadisticas
            Console.Write(textoAMostrar);
            int n = valorAnterior;
            int.TryParse(Console.ReadLine(), out n);
            if (n <= valorMinimo) return valorMinimo;
            return n;
        }
        static public bool ConfirmarSeleccion(string textoAMostrar) {
            //Estructura generica para confirmar la seleccion del usuario (lo tipico de "¿Estás seguro? (S/N)")
            Console.Write(textoAMostrar);
            Console.ReadKey(true);
            ConsoleKey key = Console.ReadKey(true).Key;
            Console.Clear();
            if (key == ConsoleKey.S) return true;
            return false;
        }
        public static Ataque[] poolMovimientos = {
            /*OBSERVACION esto me generaba dudas, pero creo que es mejor tener aqui la pool de Movimientos aqui antes que generarla en cada objeto Pokemon que se cree. 
             * Si el programa usara mas el Main(), la tendria en el main, pero el main unicamente va a llamar a Menu.MostrarMenu()*/
            new AtaqueEspecial  ("Llamarada",       110,    85,     Tipo.Fuego),
            new AtaqueFisico    ("Puño Fuego",      75,     100,    Tipo.Fuego),
            new AtaqueFisico    ("Acua Jet",        40,     100,    Tipo.Agua),
            new AtaqueEspecial  ("Hidrobomba",      120,    80,     Tipo.Agua),
            new AtaqueFisico    ("Hoja aguda",      90,     100,    Tipo.Planta),
            new AtaqueEspecial  ("Ciclon de Hojas", 65,     90,     Tipo.Planta),
            new AtaqueFisico    ("Puño Electrico",  75,     100,    Tipo.Electrico),
            new AtaqueEspecial  ("Rayo",            90,     100,    Tipo.Electrico),
            new AtaqueFisico    ("Corte",           50,     95,     Tipo.Normal),
            new AtaqueEspecial  ("Triataque",       80,     100,    Tipo.Normal),
            new AtaqueFisico    ("Bola Sombra",     80,     100,    Tipo.Fantasma),
            new AtaqueEspecial  ("Sombra vil",      40,     100,    Tipo.Fantasma),
            new AtaqueEspecial  ("Psicorrayo",      65,     100,    Tipo.Psiquico),
            new AtaqueFisico    ("Psicocolmillo",   85,     100,    Tipo.Psiquico),
            new AtaqueFisico    ("Triturar",        80,     100,    Tipo.Siniestro),
            new AtaqueEspecial  ("Pulso Umbrio",    80,     100,    Tipo.Siniestro),
            new AtaqueFisico    ("Gancho Alto",     85,     90,     Tipo.Lucha),
            new AtaqueEspecial  ("Onda Certera",    120,    70,     Tipo.Lucha)
        };
        public static ConsoleKey[] teclasMovimientos = { ConsoleKey.A, ConsoleKey.B, ConsoleKey.C, ConsoleKey.D, ConsoleKey.E, ConsoleKey.F, ConsoleKey.G, ConsoleKey.H, ConsoleKey.I, ConsoleKey.J, ConsoleKey.K, ConsoleKey.L, ConsoleKey.M, ConsoleKey.N, ConsoleKey.O, ConsoleKey.P, ConsoleKey.Q, ConsoleKey.R };
        public static string[] traducirTipoAString = { "FUEGO", "PLANTA", "AGUA", "ELECTRICO", "FANTASMA", "NORMAL", "LUCHA", "SINIESTRO", "PSIQUICO" };        
        public static ConsoleColor coloresTipo(Tipo tipo){
            //Esta funcion generica devuelve un color segun el tipo elemental del movimiento o pokemon en cuestion
            ConsoleColor[] colores = new ConsoleColor[] {ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Yellow, ConsoleColor.DarkMagenta, ConsoleColor.White, ConsoleColor.DarkRed, ConsoleColor.DarkGray, ConsoleColor.Magenta};
            return colores[(int)tipo];
        }
        //las debilidades no son exactas, ya que hay tipos con varias debilidades
        //sin embargo, cumple su objetivo, funciona para una "IA" muy basica en la clase PokemonCPU
        public static Tipo[] debilidades = {Tipo.Agua, Tipo.Fuego, Tipo.Agua, Tipo.Fuego, Tipo.Siniestro, Tipo.Lucha, Tipo.Psiquico, Tipo.Lucha, Tipo.Fantasma};
    }
}