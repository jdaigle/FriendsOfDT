<?php

function dbcon() {
// MySQL configuration
//global $CONFIG;
$CONFIG['dbserver'] = "localhost";    // Your database server
$CONFIG['dbuser'] = "SECRET";        // Your mysql username
$CONFIG['dbpass'] = "SECRET";   // Your mysql password
$CONFIG['dbname'] = "dramatech";    // Your mysql database name

$result = @mysql_connect($CONFIG['dbserver'], $CONFIG['dbuser'], $CONFIG['dbpass']);
if (!$result) {
   die ('Error connecting to mysql');
   return false;
}
if (!mysql_select_db($CONFIG['dbname']))
   return false;
   return $result;
}

?>

