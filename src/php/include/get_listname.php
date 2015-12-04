<?
function get_listname($peep_id) {
    $peep_select = mysql_query("SELECT lname,fname,mname,nickname FROM people WHERE ID=$peep_id");
    while ($result = mysql_fetch_array($peep_select)) {
        $listname=stripslashes("$result[0]");
        if (strlen($result[1]) > 0) {$listname=stripslashes("$listname, $result[1]");} #First Name
        if (strlen($result[2]) > 0) {$listname=stripslashes("$listname $result[2]");} #Middle Name
        if (strlen($result[3]) > 0) {$listname=stripslashes("$listname \($result[3]\)");} #Nickname
    }
    return ($listname);
}

?>
