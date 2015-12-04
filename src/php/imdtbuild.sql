-- MySQL dump 10.9
--
-- Host: localhost    Database: dramatech
-- ------------------------------------------------------
-- Server version	4.1.20

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `EC`
--

DROP TABLE IF EXISTS `EC`;
CREATE TABLE `EC` (
  `ID` int(11) NOT NULL auto_increment,
  `peepID` int(11) default NULL,
  `ECID` tinyint(4) default NULL,
  `year` smallint(6) default NULL,
  `last_mod` date default NULL,
  PRIMARY KEY  (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;


--
-- Table structure for table `EC_bkup`
--

DROP TABLE IF EXISTS `EC_bkup`;
CREATE TABLE `EC_bkup` (
  `ID` int(11) default NULL,
  `peepID` int(11) default NULL,
  `ECID` tinyint(4) default NULL,
  `year` smallint(6) default NULL,
  `last_mod` date default NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Table structure for table `EC_list`
--

DROP TABLE IF EXISTS `EC_list`;
CREATE TABLE `EC_list` (
  `ID` int(11) NOT NULL auto_increment,
  `title` varchar(50) default NULL,
  PRIMARY KEY  (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Dumping data for table `EC_list`
--


/*!40000 ALTER TABLE `EC_list` DISABLE KEYS */;
LOCK TABLES `EC_list` WRITE;
INSERT INTO `EC_list` VALUES (1,'President'),(2,'Business Manager'),(3,'Production Manager'),(4,'Marketing Director'),(5,'Secretary'),(6,'Member At Large'),(7,'Historian'),(8,'Producer'),(9,'Associate Producer'),(10,'Faculty Advisor'),(11,'LTT! Leader'),(12,'LTT! Publicity Wolf'),(13,'LTT! Production Manager'),(14,'LTT! Secretary'),(15,'LTT! Historian'),(16,'Technical Services Committee');
UNLOCK TABLES;
/*!40000 ALTER TABLE `EC_list` ENABLE KEYS */;

--
-- Table structure for table `EC_list_bkup`
--

DROP TABLE IF EXISTS `EC_list_bkup`;
CREATE TABLE `EC_list_bkup` (
  `ID` int(11) NOT NULL default '0',
  `title` varchar(50) default NULL,
  `last_mod` date default NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;


--
-- Table structure for table `awards`
--

DROP TABLE IF EXISTS `awards`;
CREATE TABLE `awards` (
  `ID` int(11) NOT NULL auto_increment,
  `showID` int(11) default NULL,
  `peepID` int(11) default NULL,
  `awardID` tinyint(4) default NULL,
  `year` smallint(6) default NULL,
  `last_mod` date default NULL,
  PRIMARY KEY  (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Table structure for table `awards_bkup`
--

DROP TABLE IF EXISTS `awards_bkup`;
CREATE TABLE `awards_bkup` (
  `ID` int(11) NOT NULL default '0',
  `showID` int(11) default NULL,
  `peepID` int(11) default NULL,
  `awardID` tinyint(4) default NULL,
  `year` smallint(6) default NULL,
  `last_mod` date default NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;


--
-- Table structure for table `awards_list`
--

DROP TABLE IF EXISTS `awards_list`;
CREATE TABLE `awards_list` (
  `ID` int(11) NOT NULL auto_increment,
  `name` varchar(50) default NULL,
  PRIMARY KEY  (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Dumping data for table `awards_list`
--


/*!40000 ALTER TABLE `awards_list` DISABLE KEYS */;
LOCK TABLES `awards_list` WRITE;
INSERT INTO `awards_list` VALUES (1,'Best Show'),(2,'Best Poster Design'),(3,'Best Program Design'),(4,'Best Props Design'),(5,'Best Sound Design'),(6,'Best Costume Design'),(7,'Best Male Ensemble'),(8,'Best Female Ensemble'),(9,'Best Set Design'),(10,'Best Light Design'),(11,'Best Supporting Actor'),(12,'Best Supporting Actress'),(13,'Variety Tech MVP'),(14,'Let\'s Try This! Rookie of the Year'),(15,'Let\'s Try This! Lifetime Achievement'),(16,'Best Actor'),(17,'Best Actress'),(18,'DramaTech Scholarship'),(19,'Rookie of the Year'),(20,'Dean James E. Dull Service Award'),(21,'William C. Landolina Meritorious Service Award'),(22,'Mary Nell Santacroce Meritorious Service Award'),(23,'Best Studio Show'),(24,'Best Male in a Studio Show'),(25,'Best Female in a Studio Show'),(26,'Best Technical Achievement in a Studio Show'),(27,'Best Stage Management Crew'),(28,'Best Cameo Performance');
UNLOCK TABLES;
/*!40000 ALTER TABLE `awards_list` ENABLE KEYS */;

--
-- Table structure for table `awards_list_bkup`
--

DROP TABLE IF EXISTS `awards_list_bkup`;
CREATE TABLE `awards_list_bkup` (
  `ID` int(11) NOT NULL default '0',
  `name` varchar(50) default NULL,
  `last_mod` date default NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Table structure for table `cast`
--

DROP TABLE IF EXISTS `cast`;
CREATE TABLE `cast` (
  `ID` int(11) NOT NULL auto_increment,
  `peepID` int(11) default NULL,
  `showID` int(11) default NULL,
  `role` varchar(75) default NULL,
  `last_mod` date default NULL,
  PRIMARY KEY  (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;


--
-- Table structure for table `cast_bkup`
--

DROP TABLE IF EXISTS `cast_bkup`;
CREATE TABLE `cast_bkup` (
  `ID` int(11) NOT NULL default '0',
  `peepID` int(11) default NULL,
  `showID` int(11) default NULL,
  `role` varchar(75) default NULL,
  `last_mod` date default NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;


--
-- Table structure for table `crew`
--

DROP TABLE IF EXISTS `crew`;
CREATE TABLE `crew` (
  `ID` int(11) NOT NULL auto_increment,
  `peepID` int(11) default NULL,
  `showID` int(11) default NULL,
  `jobID` int(11) default NULL,
  `last_mod` date default NULL,
  PRIMARY KEY  (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;


--
-- Table structure for table `crew_bkup`
--

DROP TABLE IF EXISTS `crew_bkup`;
CREATE TABLE `crew_bkup` (
  `ID` int(11) NOT NULL default '0',
  `peepID` int(11) default NULL,
  `showID` int(11) default NULL,
  `jobID` int(11) default NULL,
  `last_mod` date default NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;


--
-- Table structure for table `jobs`
--

DROP TABLE IF EXISTS `jobs`;
CREATE TABLE `jobs` (
  `ID` int(11) NOT NULL auto_increment,
  `job` varchar(30) default NULL,
  `priority` smallint(6) default NULL,
  `URL` varchar(50) default NULL,
  PRIMARY KEY  (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Dumping data for table `jobs`
--


/*!40000 ALTER TABLE `jobs` DISABLE KEYS */;
LOCK TABLES `jobs` WRITE;
INSERT INTO `jobs` VALUES (1,'Producer',1,NULL),(2,'Assistant Producer',2,NULL),(3,'Production Manager',7,NULL),(4,'Assistant Production Manager',8,NULL),(5,'Director',4,''),(6,'Assistant Director',5,NULL),(7,'Assistant to the Director',6,NULL),(8,'Technical Director',9,NULL),(9,'Technical Director (Acting)',10,NULL),(10,'Assistant Director/Movement',13,NULL),(11,'Artistic Director',14,NULL),(12,'Music Director',17,NULL),(13,'Assistant Music Director',18,NULL),(14,'Choreographer',19,NULL),(15,'Combat Choreography',22,NULL),(16,'Stage Manager',24,NULL),(17,'Assistant Stage Manager',25,NULL),(18,'Rehearsal Stage Manager',26,NULL),(19,'Production Stage Manager',27,NULL),(20,'Line Coach',28,NULL),(21,'Set Designer',30,NULL),(22,'Set/Visual Designer',31,NULL),(23,'Assistant Set Designer',32,NULL),(24,'Scenic Designer',33,NULL),(25,'Assistant Scenic Designer',34,NULL),(26,'Scenic Artist',35,NULL),(27,'Master Carpenter',36,NULL),(28,'Apprentice Master Carpenter',37,NULL),(29,'Assistant Master Carpenter',38,NULL),(30,'Set Construction Crew Chief',39,NULL),(31,'Carpenter',40,NULL),(32,'Assistant Carpenter',41,NULL),(33,'Set Decorations Crew Chief',43,NULL),(34,'Visual Constructant',44,NULL),(35,'Artist',45,NULL),(36,'Set Decorations',46,NULL),(37,'Set Construction',47,NULL),(38,'Guest Design Consultant',48,NULL),(39,'Lighting Designer',49,NULL),(40,'Assistant Light Designer',50,NULL),(41,'Lighting Director',51,NULL),(42,'Audio/Visual Design',54,NULL),(43,'Resident Master Electrician',55,NULL),(44,'Master Electrician',56,NULL),(45,'Assistant Master Electrician',57,NULL),(46,'Apprentice Master Electrician',58,NULL),(47,'Electrician',59,NULL),(48,'Assistant Electrician',60,NULL),(49,'Onstage Electrician',61,NULL),(50,'Light Crew Chief',62,NULL),(51,'Lighting',63,NULL),(52,'Light Operator',64,NULL),(53,'Operator',65,NULL),(54,'Spotlight Operator',66,NULL),(55,'Special Effects',68,NULL),(56,'Video Designer',69,NULL),(57,'Slide Software',70,NULL),(58,'Slide Art',71,NULL),(59,'Automated Light Design',72,NULL),(60,'Audio Visual Effects',74,NULL),(61,'Remote Operator',75,NULL),(62,'Cryogenic Systems',76,NULL),(63,'Side Design',77,NULL),(64,'Side Operator',78,NULL),(65,'Media Specialist',79,NULL),(66,'Sound Director',80,NULL),(67,'Sound Designer',81,NULL),(68,'Assistant Sound Designer',83,NULL),(69,'Sound Engineering',84,NULL),(70,'Effects Designer',85,NULL),(71,'Sound Consultant',87,NULL),(72,'Sound',88,NULL),(73,'Sound Operator',89,NULL),(74,'Audio Engineering',90,NULL),(75,'Technical Advisor',92,NULL),(76,'Technical Crew',93,NULL),(77,'Jukebox Music',94,NULL),(78,'Properties Designer',95,NULL),(79,'Assistant Properties Designer',96,NULL),(80,'Properties',97,NULL),(81,'Properties Master',98,NULL),(82,'Weapons Master',99,NULL),(83,'Key Grip',100,NULL),(84,'Grip',101,NULL),(85,'Flyman',102,NULL),(86,'Stagehand',103,NULL),(87,'Costume Designer',104,NULL),(88,'Assistant Costume Designer',105,NULL),(89,'Costume Mistress',106,NULL),(90,'Assistant Costumer',109,NULL),(91,'Costume Consultant',110,NULL),(92,'Costumes',112,NULL),(93,'Costume Assistant',113,NULL),(94,'Makeup Designer',115,NULL),(95,'Makeup Crewchief',117,NULL),(96,'Makeup',118,NULL),(97,'Hair',119,NULL),(98,'Hair and Wigs',120,NULL),(99,'Publicity Director',121,NULL),(100,'Head Publicist',122,NULL),(101,'Festival Publicity Director',123,NULL),(102,'Publicity Crew Chief',124,NULL),(103,'Publicity',125,NULL),(104,'Assistant Publicity',126,NULL),(105,'Marketing Director',127,NULL),(106,'Business Manager',128,NULL),(107,'Poster Design',130,NULL),(108,'Program Designer',131,NULL),(109,'Assistant Program Designer',132,NULL),(110,'Program Layout',133,NULL),(111,'Program Art',134,NULL),(112,'Photographer',135,NULL),(113,'Typesetting',136,NULL),(114,'Marketing',137,NULL),(115,'Advertising',138,NULL),(116,'Off Campus Marketing',139,NULL),(117,'Operations Manager',140,NULL),(118,'Interactivity/Web Site',141,NULL),(119,'Graphics Design',142,NULL),(120,'Banner Design',143,NULL),(121,'House Manager',144,NULL),(122,'Box Office Manager',145,NULL),(123,'Composer',147,NULL),(124,'Original Music',148,NULL),(125,'Conductor',149,NULL),(126,'Piano',150,NULL),(127,'Keyboards',151,NULL),(128,'Synthesizer',152,NULL),(129,'Key Bass',154,NULL),(130,'Bass',155,NULL),(131,'String Bass',156,NULL),(132,'Guitar',157,NULL),(133,'Pit Guitarist',158,NULL),(134,'Banjoist',159,NULL),(135,'Trumpet',160,NULL),(136,'Trombone',161,NULL),(137,'French Horn',162,NULL),(138,'Flute',163,NULL),(139,'Clarinet',164,NULL),(140,'Tenor Saxophone',167,NULL),(141,'Soprano Saxophone',168,NULL),(142,'Bass Clarinet',169,NULL),(143,'Drums',171,NULL),(144,'Percussion',172,NULL),(145,'Orchestra',173,NULL),(146,'Musician',174,NULL),(147,'Moog Effects',175,NULL),(148,'Live Music',176,NULL),(149,'Additional Music Arrangement',177,NULL),(150,'With Special Thanks',181,NULL),(153,'Reeds',170,NULL),(152,'Microphone Operator',91,NULL),(154,'Visual Design',42,NULL),(155,'Assistant Sound Engineer',86,NULL),(156,'Violin',153,NULL),(157,'Saxophone',166,NULL),(158,'Assistant Choreographer',21,NULL),(159,'Band Roadie',179,NULL),(160,'Automated Light Operator',73,NULL),(161,'Rehearsal Pianist',178,NULL),(162,'Oboe/English Horn',165,NULL),(163,'Mask Construction',114,NULL),(164,'Assistant Make-up Designer',116,NULL),(169,'Dramaturg',16,'http://en.wikipedia.org/wiki/Dramaturg'),(166,'Illumination Engineering',53,NULL),(170,'Costume Crew Chief',108,NULL),(172,'Lighting Consultant',52,NULL),(173,'Sound Crew Chief',82,NULL),(174,'Style Coach',23,NULL),(175,'Author',3,NULL),(182,'Production Staff',180,NULL),(180,'Business Staff',129,NULL),(186,'Technical Designer',12,NULL),(184,'Prompter',29,NULL),(185,'Scenery Subcommittee',15,NULL),(187,'Wardrobe',111,NULL),(188,'Movement Director',20,NULL),(189,'Costume Master',107,NULL),(190,'Artistic Designer',11,NULL),(191,'Low Budget Ninja Spotlight Op',67,''),(192,'Theatre Maintenance',146,'');
UNLOCK TABLES;
/*!40000 ALTER TABLE `jobs` ENABLE KEYS */;

--
-- Table structure for table `jobs_bkup`
--

DROP TABLE IF EXISTS `jobs_bkup`;
CREATE TABLE `jobs_bkup` (
  `ID` int(11) NOT NULL default '0',
  `job` varchar(30) default NULL,
  `priority` smallint(6) default NULL,
  `last_mod` date default NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Table structure for table `people`
--

DROP TABLE IF EXISTS `people`;
CREATE TABLE `people` (
  `ID` int(11) NOT NULL auto_increment,
  `hon` varchar(20) default NULL,
  `fname` varchar(25) default NULL,
  `mname` varchar(25) default NULL,
  `lname` varchar(25) default NULL,
  `suffix` varchar(10) default NULL,
  `nickname` varchar(20) default NULL,
  `picture` varchar(100) default './images/nopic.gif',
  `thumb` varchar(100) default './images/nopic.gif',
  `tinypic` varchar(100) default './images/nopicind.gif',
  `bio` blob,
  `username` varchar(25) default NULL,
  `password` varchar(32) default NULL,
  `email` varchar(50) default NULL,
  `level` varchar(50) default NULL,
  `last_mod` date default NULL,
  PRIMARY KEY  (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Table structure for table `people_bkup`
--

DROP TABLE IF EXISTS `people_bkup`;
CREATE TABLE `people_bkup` (
  `ID` int(11) NOT NULL default '0',
  `hon` varchar(20) default NULL,
  `fname` varchar(25) default NULL,
  `mname` varchar(25) default NULL,
  `lname` varchar(25) default NULL,
  `suffix` varchar(10) default NULL,
  `nickname` varchar(20) default NULL,
  `picture` varchar(100) default './images/nopic.gif',
  `thumb` varchar(100) default './images/nopic.gif',
  `tinypic` varchar(100) default './images/nopicind.gif',
  `bio` blob,
  `username` varchar(25) default NULL,
  `password` varchar(25) default NULL,
  `email` varchar(50) default NULL,
  `level` varchar(50) default NULL,
  `last_mod` date default NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;


--
-- Table structure for table `q_list`
--

DROP TABLE IF EXISTS `q_list`;
CREATE TABLE `q_list` (
  `ID` tinyint(4) NOT NULL default '0',
  `quarter` varchar(10) default NULL,
  PRIMARY KEY  (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Dumping data for table `q_list`
--


/*!40000 ALTER TABLE `q_list` DISABLE KEYS */;
LOCK TABLES `q_list` WRITE;
-- INSERT INTO `q_list` VALUES (1,'Winter'),(2,'Spring'),(3,'Summer'),(4,'Fall');
INSERT INTO `q_list` VALUES (1,'January'),(2,'February'),(3,'March'),(4,'April'),(5,'May'),(6,'June'),(7,'July'),(8,'August'),(9,'September'),(10,'October'),(11,'November'),(12,'December');
UNLOCK TABLES;
/*!40000 ALTER TABLE `q_list` ENABLE KEYS */;

--
-- Table structure for table `q_list_bkup`
--

DROP TABLE IF EXISTS `q_list_bkup`;
CREATE TABLE `q_list_bkup` (
  `ID` int(11) NOT NULL default '0',
  `quarter` varchar(10) default NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Dumping data for table `q_list_bkup`
--


/*!40000 ALTER TABLE `q_list_bkup` DISABLE KEYS */;
LOCK TABLES `q_list_bkup` WRITE;
UNLOCK TABLES;
/*!40000 ALTER TABLE `q_list_bkup` ENABLE KEYS */;

--
-- Table structure for table `shows`
--

DROP TABLE IF EXISTS `shows`;
CREATE TABLE `shows` (
  `ID` int(11) NOT NULL auto_increment,
  `title` varchar(100) default NULL,
  `quarter` tinyint(4) default NULL,
  `author` varchar(100) default NULL,
  `year` smallint(6) default NULL,
  `thumb` varchar(100) default './images/noposter.gif',
  `poster` varchar(100) default './images/noposter.gif',
  `tinypic` varchar(100) default './images/tinyposter.gif',
  `pictures` varchar(100) default NULL,
  `funfacts` blob,
  `last_mod` date default NULL,
  PRIMARY KEY  (`ID`)
) ENGINE=MyISAM DEFAULT CHARSET=latin1;


--
-- Table structure for table `shows_bkup`
--

DROP TABLE IF EXISTS `shows_bkup`;
CREATE TABLE `shows_bkup` (
  `ID` int(11) NOT NULL default '0',
  `title` varchar(100) default NULL,
  `quarter` tinyint(4) default NULL,
  `author` varchar(100) default NULL,
  `year` smallint(6) default NULL,
  `thumb` varchar(100) default './images/noposter.gif',
  `poster` varchar(100) default './images/noposter.gif',
  `tinypic` varchar(100) default './images/tinyposter.gif',
  `pictures` varchar(100) default NULL,
  `funfacts` blob,
  `last_mod` date default NULL
) ENGINE=MyISAM DEFAULT CHARSET=latin1;

--
-- Dumping data for table `shows_bkup`
--


/*!40000 ALTER TABLE `shows_bkup` DISABLE KEYS */;
LOCK TABLES `shows_bkup` WRITE;
UNLOCK TABLES;
/*!40000 ALTER TABLE `shows_bkup` ENABLE KEYS */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

