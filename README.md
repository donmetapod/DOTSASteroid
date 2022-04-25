# DOTSASteroid

Mi primer proyecto utilizando **DOTS** creado en 7 días

Lo realicé utilizando DOTS versión 0.5 que es la actulización más grande que ha tenido DOTS desde que fue anunciado.

El juego es un clon del clásico Asteroids pero con algunas variantes en el diseño para hacer las sesiones de juego más cortas.

La nave principal que puedes controlar con las teclas A y D para rotar y la tecla W para agregar impulso (que por ahora no está limitado), 
esta nave (objeto _Spaceship_) está controlada por **SpaceshipSystem** que hace uso de **SpaceshipData** para su configuración, es
un sistema que incluye el movimiento de la nave, recepción del Input que utiliza un componente llamado **InputData**.

Esta estructura de utilizar una clase que actúa como sistema y una clase que actúa sobre las entidades la utilizo en casi todo el proyecto ya que
me pareció una estructura sencilla de entender, no se si es la más óptima ya que nunca antes había hecho un proyecto en DOTS y como solo tenía
7 días para hacerlo no contemplé optimización o mejores prácticas (que según las fuentes oficiales aún no existen) sino que me enfoqué en aprender
las funcionalidades más básicas que me permitieran realizar el juego y así mantenerlo tan simple como sea posible.

Similar al Spaceship, los asteroides del juego tienen un sistema llamado **AsteroidMovementSystem** que genera su comportamiento y se ayuda del 
componente **AsteroidData** que guarda su configuración todo el tiempo, los asteroides son instancias nuevas de Entidades que se están creando cada 
cierto tiempo, no utilicé un pool de asteroides (cosa que si hice con las balas del juego) para poder aprender y practicar la creación y destrucción 
de instancias de entidades en la escena del juego.

La creación de las instancias de asteroides está a cargo de **SpawnerMono**, si, estoy creando instancias de entidades desde un **Monobehaviour**, también hice un
spawner con un sistema de ECS pero quería experimentar para ver si esto era posible (combinar Monobehaviours con sistemas) y al menos en las pocas 
pruebas que pude hacer parece estar funcionando muy bien. **SpawnerMono** además se reutiliza para crear PowerUps en el juego.

La destrucción de los asteroides se realiza en **OnTriggerSystem** que revisa todas las interacciones de física del juego, algo que la verdad me
costó un poco de trabajo entender porque funciona muy distinto al sistema "tradicional" de colisiones de Unity, investigué un poco y al parecer
este sistema puede cambiar mucho en el próximo año, al estar utilizando la versión 0.5 de DOTS todo está propenso a cambio, de hecho los sistemas
de audio, animación y navegación de Unity que estaban siendo desarrollados para DOTS ya no están en desarrollo a partir de esta versión, al parecer
van a reinicar su desarrollo o al menos van a cambiar de manera significativa de acuerdo a Unity. Regresando al tema de OnTriggerSystem, este sistema
registra toas las interacciones del juego todo el tiempo y reporta cuando dos entidades (EntityA y EntityB) interactúan, lo interesante es que
no es posible saber de antemano cual de los dos objetos que interactuaron es EntityA y cual es EntityB lo que provoca que se tenga que hacer una
revisión dentro de una lista de componentes que coinciden a los que tienen estas dos entidades, es algo exaustivo pero así es como funciona al 
menos hasta el momento. En el caso de los asteroides se revisa si entraron en contacto con otra entidad que pertenezca a las balas del jugador
de ser así, se revisa cual de las dos entidades es el asteroide para poder destruirlo.

Todas las interacciones con física del juego siguen un proceso similar al descrito para los asteroides...

A diferencia de los asteroides donde se crean y se destruyen entidades, para las balas existe un pool, bueno, en realidad son dos ya que separé 
las balas del jugador de las balas del UFO, por una lado pensé un poco en la óptimización pero más que nada quería experimentar crear un 
sistema de pooling en ECS, la activación de las balas la realiza **SpaceshipSystem** ya que lee cuando el usuario quiere disparar y estas
regresan al pool gracias al **BulletSystem** lo cual me resultó interesante porque son dos sistemas por separado afectando las mismas entidades y 
fue simple de lograr y funciona bastante bien.

Otro sistema interesante es el de **UFOSystem** ya que aunque es una IA muy básica me costó un poco de trabajo en especial porque siempre pienso este 
tipo de comportamientos como componentes en Monobehaviours y aquí fue un proceso muy distinto.

En el proyecto existen además otros sistemas pequeños como el **WarpSystem** que permite a entidades salir por un lado de la pantalla
y llegar por el otro, el sistema de autodestrucción (**SelfDestroySystem**) que permite asignar un tiempo de vida a entidades y las destruye después de que pasa ese
tiempo y también **RotatorSystem** que simplemente rota entidades en cualquiera de sus ejes.

Un experimento que hice y que funcionó (aunque aún no me encanta) es tener un **GameManager** desde Monobehaviour, lo hice por dos principales razones
la primera es que quería tener control desde un ambiente que me resultara más familiar y la segunda es que cuando empecé a investigar sobre DOTS y me 
enteré de que se había pausado el desarrollo de la parte de Audio pensé que iba a ser mejor idea simplemente hacer esa parte del lado de Monobehaviour
para no meterme en problemas o aprender algo que pronto va a estar descontinuado, si bien no me encantó la implementación si vi que era muy conveniente, al 
grado de que también me sirvió para la implementación de la UI que es muy sencilla ya que solo muetras el marcador, las vidas y una pantalla de GameOver
al perder todas las vidas.

Sin duda este proyecto estuvo muy interesante, el proceso de "desaprender" a lo que uno está acostumbrado y aprender algo nuevo siempre es doloroso 
pero termina siendo muy gratificante cuando las cosas empiezan a hacer sentido, mi siguiente paso es el que siempre recomiendo, dar una segunda 
iteración sobre lo aprendido, limpiar, refactorizar, estudiar formas distintas de lograr lo mismo para incrementar las habilidades y poco a poco
poder ser capaz de crear proyectos más escalables, optimizados y con mejores prácticas, en lo personal me gustaría empezar a leer y practicar más sobre el
uso del Job System ya que no me dió mucho tiempo para aprender sobre este durante la semana que desarrolle este proyecto.
