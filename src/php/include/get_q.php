<?
function get_q ($q) {
    check_number($q);
    $q_select = mysql_query("select quarter from q_list where ID = $q");
    while ($q_return =  mysql_fetch_array($q_select)) { $qname=$q_return[0]; }
    return($qname);
}
?>
