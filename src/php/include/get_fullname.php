<?
function get_fullname($peep_id) {
    $peep_select = mysql_query("select hon,fname,mname,nickname,lname,suffix from people where ID=$peep_id");
    while ($result = mysql_fetch_array($peep_select)) {
        $fullname=stripslashes("$result[1]");
        if (strlen($result[2]) > 0 ) {$fullname=stripslashes("$fullname $result[2]");} #Middle Name
        if (strlen($result[3]) > 0) {$fullname=stripslashes("$fullname \'$result[3]\'");} #Nickname
        if (strlen($result[4]) > 0) {$fullname=stripslashes("$fullname $result[4]");} #Last Name
        if (strlen($result[5]) > 0) {$fullname=stripslashes("$fullname, $result[5]");} #Suffix
        if (strlen($result[0]) > 0) {$fullname=stripslashes("$result[0] $fullname");} #Honorific
    }
    return ($fullname);
}
?>
